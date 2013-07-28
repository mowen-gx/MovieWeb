using System;

namespace MovieWeb.Model{
	 	//MovieSynTime
		public class MovieSynTime
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
		/// MovieGuid
        /// </summary>		
		private string _movieguid;
        public string MovieGuid
        {
            get{ return _movieguid; }
            set{ _movieguid = value; }
        }        
		/// <summary>
		/// BaseSysTime
        /// </summary>		
		private DateTime _basesystime;
        public DateTime BaseSysTime
        {
            get{ return _basesystime; }
            set{ _basesystime = value; }
        }        
		/// <summary>
		/// StarTime
        /// </summary>		
		private DateTime _startime;
        public DateTime StarTime
        {
            get{ return _startime; }
            set{ _startime = value; }
        }        
		   
	}
}

