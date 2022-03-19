namespace MonMoose.Core.DataExporter
{
    public abstract class DataLoaderExporter
    {
        protected DataLoaderExportContext m_context;

        public void Export(DataLoaderExportContext context)
        {
            m_context = context;
            OnExport();
        }

        protected abstract void OnExport();
    }
}