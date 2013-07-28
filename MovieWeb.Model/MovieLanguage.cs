namespace MovieWeb.Model{
	 	//MovieLanguage
		public class MovieLanguage
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
		/// LanguageGuid
        /// </summary>		
		private string _languageguid;
        public string LanguageGuid
        {
            get{ return _languageguid; }
            set{ _languageguid = value; }
        }        
		   
	}
}

