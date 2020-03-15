namespace STP.Profile.Domain.FilterModels
{
    public class BaseFilterModel
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public BaseFilterModel()
        {
            Skip = 0;
            Take = 10;
        }
    }
}
