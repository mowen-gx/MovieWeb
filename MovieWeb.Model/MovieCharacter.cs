namespace MovieWeb.Model{
	 	//MovieCharacter
		public class MovieCharacter
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
		/// CharacterGuid
        /// </summary>		
		private string _characterguid;
        public string CharacterGuid
        {
            get{ return _characterguid; }
            set{ _characterguid = value; }
        }        
		   
	}
}

