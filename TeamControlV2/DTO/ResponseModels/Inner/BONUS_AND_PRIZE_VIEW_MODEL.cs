namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class BONUS_AND_PRIZE_VIEW_MODEL
    {
        public int Id { get; set; }

        public string Employee { get; set; }

        public string Date { get; set; }

        public string Amount { get; set; }

        public string Reason { get; set; }
        
        public string Project { get; set; }

        public bool IsPrize { get; set; }

    }
}
