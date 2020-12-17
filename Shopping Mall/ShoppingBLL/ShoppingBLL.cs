   
                                                            
using System;                                                                                           
using System.Collections.Generic;                                                                                             
using System.Linq;                                                                                                                        
using System.Text;                                                                                                   
using ShoppingDAL;                                                                                                                                         
namespace ShoppingBLL                                                                                         
{                                                                             
	public partial class achievement : BaseBLL<ShoppingModel.achievement>
    {
		public override void SetDAL()
		{     
			idal =new achievementDAL(); 
		}             
    }                       
	public partial class course : BaseBLL<ShoppingModel.course>
    {
		public override void SetDAL()
		{     
			idal =new courseDAL(); 
		}             
    }                       
	public partial class students : BaseBLL<ShoppingModel.students>
    {
		public override void SetDAL()
		{     
			idal =new studentsDAL(); 
		}             
    }                       
	public partial class sysdiagrams : BaseBLL<ShoppingModel.sysdiagrams>
    {
		public override void SetDAL()
		{     
			idal =new sysdiagramsDAL(); 
		}             
    }                       
        
}                                                 