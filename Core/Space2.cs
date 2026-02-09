using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace FGUFW
{
    /// <summary>
    /// 空间划分 点模式
    /// 空间由X*Y个格子构成 格子顺序X >> Y
    /// 点在格子内: gridMin >= 点 < gridMax
    /// 空间之外点的所在格子索引为-1
    /// </summary>
    [BurstCompile]
    public struct Space2
    {
        // [BurstCompile]
        public static NativeStream PointsInSpace(in NativeArray<float2> points,in float2 spaceSize,in int2 spaceMaxIndex,in float2 spaceCenter,Allocator allocator=Allocator.Temp)
        {
            var stream = new NativeStream(spaceMaxIndex.x*spaceMaxIndex.y,allocator);
            var writer = stream.AsWriter();

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var gridIndex = IndexOf(in point,in spaceSize,in spaceMaxIndex,in spaceCenter);
                writer.BeginForEachIndex(gridIndex);
                writer.Write(i);
                writer.EndForEachIndex();
            }

            return stream;
        }

        public static NativeStream PointsInSpace(in NativeArray<float2> points,in NativeArray<float> radius,in float2 spaceSize,in int2 spaceMaxIndex,in float2 spaceCenter,Allocator allocator=Allocator.Temp)
        {
            var stream = new NativeStream(spaceMaxIndex.x*spaceMaxIndex.y,allocator);
            var writer = stream.AsWriter();

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var r = radius[i];
                var gridIndexs = OverlapGrids(in point,r,in spaceSize,in spaceMaxIndex,in spaceCenter);
                foreach (var gridIndex in gridIndexs)
                {
                    writer.BeginForEachIndex(gridIndex);
                    writer.Write(i);
                    writer.EndForEachIndex();
                }
            }

            return stream;
        }

        // [BurstCompile]
        public static NativeArray<int> OverlapGrids(in float2 point,float radius,in float2 spaceSize,in int2 spaceMaxIndex,in float2 spaceCenter,Allocator allocator=Allocator.Temp)
        {
            Bounds2 bounds = new Bounds2(point,new float2(radius*2,radius*2));
            Bounds2 spaceBounds = new Bounds2(spaceCenter,spaceSize);
            float2 gridSize = spaceSize/spaceMaxIndex;
            return OverlapGrids(gridSize,bounds,spaceBounds,spaceMaxIndex,allocator);
        }

        public static NativeArray<int> OverlapGrids(float2 girdSize, Bounds2 bounds,Bounds2 spaceBounds,int2 spaceMaxIndex,Allocator allocator=Allocator.Temp)
        {
            int2 boxInGridSize = int2.zero;
            int2 boxInGridIndex = int2.zero;

            var (overlap,overlapBounds) = spaceBounds.Overlap(bounds);
            if(!overlap)return default;
            var min = overlapBounds.Min;
            var size = overlapBounds.Size;

            var _spaceMaxPoint = spaceBounds.Max;
            var _spaceMinPoint = spaceBounds.Min;
            
            boxInGridIndex.x = MathHelper.IndexOf(spaceMaxIndex.x,min.x,_spaceMaxPoint.x-_spaceMinPoint.x,_spaceMinPoint.x);
            boxInGridIndex.y = MathHelper.IndexOf(spaceMaxIndex.y,min.y,_spaceMaxPoint.y-_spaceMinPoint.y,_spaceMinPoint.y);

            float length = 0;
            length = (size.x - (boxInGridIndex.x+1)*girdSize.x - min.x)/girdSize.x;
            boxInGridSize.x = length<0?1:2+(int)(length/girdSize.x);

            length = (size.y - (boxInGridIndex.y+1)*girdSize.y - min.y)/girdSize.y;
            boxInGridSize.y = length<0?1:2+(int)(length/girdSize.y);


            NativeArray<int> ls;
            int count = boxInGridSize.x*boxInGridSize.y;
            if(
                bounds.Min.x<spaceBounds.Min.x ||
                bounds.Min.y<spaceBounds.Min.y ||
                
                bounds.Max.x>=spaceBounds.Max.x ||
                bounds.Max.y>=spaceBounds.Max.y 
            )
            {
                ls = new NativeArray<int>(count+1,allocator);
                ls[count] = -1;
            }
            else
            {
                ls = new NativeArray<int>(count,allocator);
            }

            for (int y = 0,i=0; y < boxInGridSize.y; y++)
            {
                for (int x = 0; x < boxInGridSize.x; x++,i++)
                {
                    ls[i] = getGridIndex(spaceMaxIndex,boxInGridIndex.x+x,boxInGridIndex.y+y);
                }
            }

            return ls;
        }

        private static int getGridIndex(int2 spaceMaxIndex,int x,int y)
        {
            return y*spaceMaxIndex.x + x;
        }

        [BurstCompile]
        public static int IndexOf(in float2 point,in float2 spaceSize,in int2 spaceMaxIndex,in float2 spaceCenter)
        {
            int idx = -1;

            float2 spaceMin = spaceCenter-spaceSize/2;
            float2 p = point-spaceMin;

            int idx_x = indexOf(p.x,spaceSize.x,spaceMaxIndex.x);
            if(idx_x==-1)return idx;

            int idx_y = indexOf(p.y,spaceSize.y,spaceMaxIndex.y);
            if(idx_x==-1)return idx;

            idx = idx_x + idx_y*spaceMaxIndex.x;

            return idx;
        }

        [BurstCompile]
        private static int indexOf(float p,float length,int count)
        {
            int idx = -1;
            if(p<0 || p>=length)return idx;
            idx = (int)(p/length*count);
            return idx;
        }

        public static ValueTuple<bool,float2> LineOverlap(float2 l1,float2 l2)
        {
            var overlap = float2.zero;
            float start_1 = l1.x;
            float length_1 = l1.y;
            float start_2 = l2.x;
            float length_2 = l2.y;

            if(start_1==start_2)
            {
                return (true,new float2(start_2,math.min(start_1+length_1,start_2+length_2)-start_2));
            }
            else if(start_1<start_2)
            {
                if(start_2 < start_1+length_1)
                {
                    return (true,new float2(start_2,math.min(start_1+length_1,start_2+length_2)-start_2));
                }
            }
            else
            {
                if(start_1 < start_2+length_2)
                {
                    return (true,new float2(start_1,math.min(start_2+length_2,start_1+length_1)-start_1));
                }
            }

            return (false,overlap);
        }

    }

    public struct Bounds2
    {
        public float2 Center;
        public float2 Extents;
        public readonly float2 Size => Extents*2;
        public readonly float2 Min => Center - Extents;
        public readonly float2 Max => Center + Extents;

        public Bounds2(float2 center,float2 size)
        {
            Center = center;
            Extents = size*0.5f;
        }

        
        public ValueTuple<bool,Bounds2> Overlap(Bounds2 other)
        {
            float2 min_1 = Min;
            float2 size_1 = Size;
            var l_x_1 = new float2(min_1.x,size_1.x);
            var l_y_1 = new float2(min_1.y,size_1.y);

            float2 min_2 = Min;
            float2 size_2 = Size;
            var l_x_2 = new float2(min_2.x,size_2.x);
            var l_y_2 = new float2(min_2.y,size_2.y);

            var (overlapX,l_x_3) = Space2.LineOverlap(l_x_1,l_x_2);
            var (overlapY,l_y_3) = Space2.LineOverlap(l_y_1,l_y_2);

            if(overlapX && overlapY)
            {
                var size = new float2(l_x_3.y , l_y_3.y);
                var min = new float2(l_x_3.x , l_y_3.x);
                var Bounds2 = new Bounds2(min+size*0.5f,size);
                return (true,Bounds2);
            }
            return (false,default(Bounds2));
        }
        
    }
}