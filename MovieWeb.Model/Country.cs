﻿namespace MovieWeb.Model{
	 	//Country
		public class Country
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
		   
	}
}

