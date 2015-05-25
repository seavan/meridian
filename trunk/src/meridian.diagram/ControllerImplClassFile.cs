namespace meridian.diagram
{
    public class ControllerImplClassFile : ClassFile
    {
        public ControllerImplClassFile(Controller _controller)
        {
            m_Controller = _controller;
            ClassName = "admin_" + _controller.Backend + "Controller";
            Inherits = _controller.Name;
        }

        public override void Functions(CSharpWriter _writer)
        {
            /*
                     protected override admin.db.IDataService<news> GetService()
        {
            return Meridian.Default.newsStore;
        }
             */
            base.Functions(_writer);
        }

        private Controller m_Controller;
    }
}