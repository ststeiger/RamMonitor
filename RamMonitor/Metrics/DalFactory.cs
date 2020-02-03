
#if false

namespace RamMonitor
{
    
    public interface IBaseDAL
    {
        public string Con { get; set; }
    }
    
    
    public interface IReadDal
        :IBaseDAL
    {

        public string GetData()
        {
            System.Console.Write("Get Data: ");
            return "abc";
        }

    }
    
    public interface IWriteDal
        :IBaseDAL
    {

        public void WriteData(string str)
        {
            System.Console.WriteLine("Write Data: " + str);
        }

    }

    public interface IReadWriteDal
        : IReadDal, IWriteDal
    {
    }


    public static class mmmm
    {
        
        public static void Log(this IWriteDal foo, System.Exception ex)
        {
            foo.WriteData( ex.ToString());        
        }
        
    }

    public class DalFields
    {
        protected string m_con;
        protected string m_ReadCon;
        protected string m_WriteCon;
        
        
        public string Con 
        {
            get { return m_con;}
            set {
                m_con = value;
            }
        }
        
    }
    
    public class MsSqlReadWriteDal
        :DalFields, IReadWriteDal
    {
        
        public MsSqlReadWriteDal()
        {
            this.m_con ="Hi";
            this.m_ReadCon ="Helloo";
            this.m_WriteCon ="Howdy";
        }

        // string IBaseDAL.Con { get; set; }
    }
    
    
    public static class DalFactory
    {
        
        
        public static void FactoryTest()
        {
            MsSqlReadWriteDal omg = new MsSqlReadWriteDal();
            
            
            // System.Console.WriteLine(omg.Con);
            System.Console.WriteLine(((IReadDal)omg).Con);
            System.Console.WriteLine(((IWriteDal)omg).Con);
            
            System.Console.WriteLine(((IReadDal)omg).GetData());
            ((IWriteDal)omg).WriteData("foobar");
            
            System.Console.WriteLine(((IReadWriteDal)omg).GetData());
            ((IReadWriteDal)omg).WriteData("foobar");
            
            
            MsSqlReadWriteDal omg2 = (MsSqlReadWriteDal) ((IReadWriteDal)omg);
            System.Console.WriteLine("omg2: ");
            System.Console.WriteLine(omg2.Con);
            
            // ((IReadWriteDal)omg).Log(1, "Hello");
            System.Console.ReadKey();
        }


        public static void Test()
        {
            FactoryTest();
        } // End Sub Main 


    } // End Class Program 
    
    
} // End Namespace RamMonitor 

#endif 
