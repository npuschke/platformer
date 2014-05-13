using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sept.tools
{
    public class Singleton
    {
        private static Singleton _instance;
        
        public static Singleton instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }
                return _instance;
            }            
        }

        private Singleton()
        {

        }

        public void makeGame()
        {

        }
    }
}
