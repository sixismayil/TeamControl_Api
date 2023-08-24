namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class SALARY_VIEW_MODEL
    {
        public int Id { get; set; }

        public string Employee { get; set; }

        public string Date { get; set; }

        public string Amount { get; set; }

        public string Salary { get; set; }

       public bool IsEdittable { get; set; }
    }
}
