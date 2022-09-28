namespace Health
{
    public abstract class Person
    {
        public string User { get; set; }
        public string Status { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is PersonJS temp)
            {
                return temp.User == User;
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            int result = 0;
            for (int i = 0; i < User.Length; i++)
            {
                result += User[i] * i;
            }
            return result;
        }
    }
}
