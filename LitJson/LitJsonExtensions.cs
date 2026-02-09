namespace LitJson
{
    public static class LitJsonExtensions
    {
        public static T ToObject<T>(this string self)
        {
            return JsonMapper.ToObject<T>(self);
        }
        
        public static string  ToJson(this object self)
        {
            return JsonMapper.ToJson(self);
        }
    }
}