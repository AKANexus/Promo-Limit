﻿namespace PromoLimit.Models
{
	public class MLInfo : EntityBase
	{
        public int UserId { get; set; }
        public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
