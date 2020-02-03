
namespace RamMonitor
{
    
    
    public class CollectingTextWriter
        : System.IO.TextWriter
    {

        protected System.Text.StringBuilder m_sb;
        
        
        public string StringValue
        {
            get
            {
                string ret = this.m_sb.ToString();
                this.m_sb.Clear();
                return ret;
            }
        }
        
        
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
        

        public CollectingTextWriter()
        {
            this.m_sb = new System.Text.StringBuilder();
        }
        
        ~CollectingTextWriter()  
        {
            this.m_sb.Clear();
            this.m_sb = null;
        } // Destructor 
        
        
        public override void Write(char value)
        {
            this.m_sb.Append(value);
        }
        
    } // End Class CollectingTextWriter
    
    
}
