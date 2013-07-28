namespace MovieWeb.Model{
	 	//MovieType
		public class MovieType
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
		/// TypeGuid
        /// </summary>		
		private string _typeguid;
        public string TypeGuid
        {
            get{ return _typeguid; }
            set{ _typeguid = value; }
        }        
		   
	}
}

