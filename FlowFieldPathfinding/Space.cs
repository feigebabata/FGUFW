using System;
using System.Collections;
using System.Collections.Generic;
using FGUFW;
using UnityEngine;

namespace FGUFW.FlowFieldPathfinding
{
    public unsafe sealed class Space
    {
        private readonly Grid[] _grids;
        public readonly Vector3Int GridCount;
        public readonly Vector3 Min,Max;
        public int EndIndex{get;private set;}

        private Space(){}

        public Space(Vector3Int girdCount,Vector3 space_min,Vector3 space_max,Grid[] grids=null)
        {
            GridCount = girdCount;
            Min = space_min;
            Max = space_max;

            int length = girdCount.x*girdCount.y*girdCount.z;
            if(grids==null || grids.Length!=length)
            {
                _grids = new Grid[length];
            }
            else
            {
                _grids = grids;
            }
        }

        public Grid this[int index]
        {
            get
            {
                if(index>=0 && index<_grids.Length)
                {
                    return _grids[index];
                }
                throw new IndexOutOfRangeException("索引不对");
            }
            set
            {
                if(value && index>=0 && index<_grids.Length)
                {
                    _grids[index]=value;
                    return;
                }
                throw new IndexOutOfRangeException("索引不对");
            }
        }

        public Grid this[Vector3Int coord]
        {
            get
            {
                int index = GeometryHelper.SpaceCoord2Index(GridCount,coord);
                return this[index];
            }
            set
            {
                int index = GeometryHelper.SpaceCoord2Index(GridCount,coord);
                this[index] = value;
            }
        }

        public Grid this[Vector3 point]
        {
            get
            {
                int index = GeometryHelper.SpacePoint2Index(Min,Max,point,GridCount);
                return this[index];
            }
            set
            {
                int index = GeometryHelper.SpacePoint2Index(Min,Max,point,GridCount);
                this[index] = value;
            }
        }

        public Space Clone()
        {
            var grids = new Grid[_grids.Length];
            Array.Copy(_grids,grids,_grids.Length);
            return new Space(GridCount,Min,Max,grids);
        }

        public bool Flow(Vector3 point,int landforms)
        {
            var start = GeometryHelper.SpacePoint2Coord(Min,Max,point,GridCount);
            return Flow(start,landforms);
        }

        /// <summary>
        /// 根据是否允许经过地貌而生成流场
        /// </summary>
        /// <param name="end"></param>
        /// <param name="landforms"></param>
        public bool Flow(Vector3Int end,uint landforms)
        {
            var grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,end);
            if(grid_idx==-1)return false;

            EndIndex = grid_idx;
            resetGrids();

            var queueCapacity = _grids.Length;
            var queueArray = stackalloc Vector3Int[queueCapacity];
            var queue = new StackMemory_Queue<Vector3Int>(queueArray,queueCapacity);
            
            var gridNearCoords = stackalloc Vector3Int[6]; 
            var gridComplete = stackalloc bool[queueCapacity];

            _grids[grid_idx].Distance = 0;
            queue.Enqueue(end);

            //广度优先遍历所有格子
            while (queue.Count>0)
            {
                //处理当前格子附近6个格子
                var center_coord = queue.Dequeue();
                grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,center_coord);
                gridComplete[grid_idx] = true;

                gridNearCoords[0] = center_coord+Vector3Int.forward;
                gridNearCoords[1] = center_coord+Vector3Int.back;
                gridNearCoords[2] = center_coord+Vector3Int.left;
                gridNearCoords[3] = center_coord+Vector3Int.right;
                gridNearCoords[4] = center_coord+Vector3Int.up;
                gridNearCoords[5] = center_coord+Vector3Int.down;


                var center = _grids[grid_idx];

                for (int i = 0; i < 6; i++)
                {
                    var grid_coord = gridNearCoords[i];
                    grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,grid_coord);
                    if(grid_idx!=-1 && BitHelper.Contains(landforms,_grids[grid_idx].Landform))
                    {
                        float distance = center.Distance + _grids[grid_idx].Weight;
                        //格子未被赋值或新距离更小
                        if(_grids[grid_idx].Distance==Grid.NoneDistance || _grids[grid_idx].Distance>distance)
                        {
                            _grids[grid_idx].Distance = distance;
                        }

                        //格子未被处理完且队列未包含
                        if(!gridComplete[grid_idx] && !queue.Contains(grid_coord))
                        {
                            queue.Enqueue(grid_coord);
                        }
                    }
                }
            }
            return true;
        }

        
        // public IEnumerator Flow2(Vector3 point,int landforms)
        // {
        //     var end = GeometryHelper.SpacePoint2Coord(Min,Max,point,GridCount);
        //     var grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,end);
        //     if(grid_idx==-1)yield break;

        //     EndIndex = grid_idx;
        //     resetGrids();

        //     var queueCapacity = _grids.Length;
        //     var queue = new Queue<Vector3Int>();
            
        //     var gridNearCoords = new Vector3Int[6]; 
        //     var gridComplete = new bool[queueCapacity];

        //     _grids[grid_idx].Distance = 0;
        //     queue.Enqueue(end);

        //     //广度优先遍历所有格子
        //     while (queue.Count>0)
        //     {
        //         //处理当前格子附近6个格子
        //         var center_coord = queue.Dequeue();
        //         grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,center_coord);
        //         gridComplete[grid_idx] = true;

        //         gridNearCoords[0] = center_coord+Vector3Int.forward;
        //         gridNearCoords[1] = center_coord+Vector3Int.back;
        //         gridNearCoords[2] = center_coord+Vector3Int.left;
        //         gridNearCoords[3] = center_coord+Vector3Int.right;
        //         gridNearCoords[4] = center_coord+Vector3Int.up;
        //         gridNearCoords[5] = center_coord+Vector3Int.down;


        //         var center = _grids[grid_idx];

        //         for (int i = 0; i < 6; i++)
        //         {
        //             var grid_coord = gridNearCoords[i];
        //             grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,grid_coord);
        //             if(grid_idx!=-1 && BitHelper.Contains(landforms,_grids[grid_idx].Landform))
        //             {
        //                 float distance = center.Distance + _grids[grid_idx].Weight;
        //                 //格子未被赋值或新距离更小
        //                 if(_grids[grid_idx].Distance==Grid.NoneDistance || _grids[grid_idx].Distance>distance)
        //                 {
        //                     _grids[grid_idx].Distance = distance;
        //                 }

        //                 //格子未被处理完且队列未包含
        //                 if(!gridComplete[grid_idx] && !queue.Contains(grid_coord))
        //                 {
        //                     Debug.Log($"入队 {grid_idx:D2}");
        //                     queue.Enqueue(grid_coord);
        //                 }
        //             }
        //         }
        //         yield return null;
        //     }
        // }

        public void MoveTo(Vector3 point,Vector3 pivot,Action<Vector3Int,Grid,Vector3> callback)
        {
            var coord = GeometryHelper.SpacePoint2Coord(Min,Max,point,GridCount);
            MoveTo(coord,pivot,callback);
        }

        /// <summary>
        /// 从起点开始顺流场移动
        /// </summary>
        public void MoveTo(Vector3Int grid_coord,Vector3 pivot,Action<Vector3Int,Grid,Vector3> callback)
        {
            var grid_idx = GeometryHelper.SpaceCoord2Index(GridCount,grid_coord);
            if(grid_idx==-1)return;
            var grid = _grids[grid_idx];
            if(grid.Distance==Grid.NoneDistance)return;
            var gridSize = VectorHelper.Division(Max-Min,GridCount);
            var pivotOffset = VectorHelper.Multiply(gridSize,pivot);

            var gridNearCoords = stackalloc Vector3Int[6]; 
            bool moveEnd = false;

            do
            {
                var grid_point = GeometryHelper.SpaceCoord2Point(grid_coord,Min,gridSize,pivotOffset);
                callback(grid_coord,grid,grid_point);

                //已到终点 结束
                if(grid_idx==EndIndex)return;

                gridNearCoords[0] = grid_coord+Vector3Int.forward;
                gridNearCoords[1] = grid_coord+Vector3Int.back;
                gridNearCoords[2] = grid_coord+Vector3Int.left;
                gridNearCoords[3] = grid_coord+Vector3Int.right;
                gridNearCoords[4] = grid_coord+Vector3Int.up;
                gridNearCoords[5] = grid_coord+Vector3Int.down;

                int min_index = grid_idx;
                Vector3Int min_coord = grid_coord;
                Grid min_grid = grid;
                for (int i = 0; i < 6; i++)
                {
                    var near_coord = gridNearCoords[i];
                    var near_idx = GeometryHelper.SpaceCoord2Index(GridCount,near_coord);
                    if(near_idx==-1)continue;
                    var near_grid = _grids[near_idx];
                    if(near_grid.Distance==Grid.NoneDistance)continue;
                    if(near_grid.Distance<_grids[min_index].Distance)
                    {
                        min_index = near_idx;
                        min_coord = near_coord;
                        min_grid = near_grid;
                    }
                }

                //无法继续移动
                if(min_index==grid_idx)
                {
                    moveEnd = true;
                }
                else
                {
                    grid_idx = min_index;
                    grid_coord = min_coord;
                    grid = min_grid;
                }
            } 
            while (!moveEnd);
        }




        /// <summary>
        /// 重制所有格子
        /// </summary>
        private void resetGrids()
        {
            for (int i = 0; i < _grids.Length; i++)
            {
                _grids[i].Distance = Grid.NoneDistance;
            }
        }

    }

    public struct Grid
    {
        public Grid(uint landform,float weight)
        {
            Landform = landform;
            Weight = weight;
            Distance = NoneDistance;
        }

        /// <summary>
        /// 地形 2的指数幂 0为无效值
        /// </summary>
        public readonly uint Landform;

        /// <summary>
        /// 距离权重
        /// </summary>
        public readonly float Weight;

        /// <summary>
        /// 距离
        /// </summary>
        public float Distance;

        /// <summary>
        /// 无距离
        /// </summary>
        public const float NoneDistance = -1;

        public static implicit operator bool(Grid exists)
        {
            return exists.Landform!=0;
        }
    }
}