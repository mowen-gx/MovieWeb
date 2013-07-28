namespace MovieWeb.Model{
	 	//Movie
		public class Movie
	{
   		     
      	/// <summary>
		/// Guid
        /// </summary>		
		private string _guid;
        public string Guid
        {
            get{ return _guid; }
            set{ _guid = value; }
        }        
		/// <summary>
		/// Name
        /// </summary>		
		private string _name;
        public string Name
        {
            get{ return _name; }
            set{ _name = value; }
        }        
		/// <summary>
		/// Country
        /// </summary>		
		private string _country;
        public string Country
        {
            get{ return _country; }
            set{ _country = value; }
        }        
		/// <summary>
		/// Language
        /// </summary>		
		private string _language;
        public string Language
        {
            get{ return _language; }
            set{ _language = value; }
        }        
		/// <summary>
		/// ScreenWriter
        /// </summary>		
		private string _screenwriter;
        public string ScreenWriter
        {
            get{ return _screenwriter; }
            set{ _screenwriter = value; }
        }        
		/// <summary>
		/// MainPic
        /// </summary>		
		private string _mainpic;
        public string MainPic
        {
            get{ return _mainpic; }
            set{ _mainpic = value; }
        }        
		/// <summary>
		/// Publish
        /// </summary>		
		private string _publish;
        public string Publish
        {
            get{ return _publish; }
            set{ _publish = value; }
        }        
		/// <summary>
		/// IsSyn
        /// </summary>		
		private int _issyn;
        public int IsSyn
        {
            get{ return _issyn; }
            set{ _issyn = value; }
        }        
		/// <summary>
		/// Era
        /// </summary>		
		private string _era;
        public string Era
        {
            get{ return _era; }
            set{ _era = value; }
        }        
		   
	}
}

