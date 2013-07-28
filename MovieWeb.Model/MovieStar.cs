namespace MovieWeb.Model{
	 	//MovieStar
		public class MovieStar
	{
   		     
      	/// <summary>
		/// MovieGuid
        /// </summary>		
		private string _movieguid;
        public string MovieGuid
        {
            get{ return _movieguid; }
            set{ _movieguid = value; }
        }        
		/// <summary>
		/// Star
        /// </summary>		
		private int _star;
        public int Star
        {
            get{ return _star; }
            set{ _star = value; }
        }        
		   
	}
}

