using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using meridian.core;
using meridian.diagram;

namespace meridian.tests.console
{
    class Program
    {
        static void ZakonTests()
        {
            //var diagramContext = new MsSqlDiagramContext("Data Source=fr.zakon.etcetera.ws;Initial Catalog=zakon_dev;Integrated Security=False;User Id=sa;Password=futari_1556");
            var diagramContext = new MsSqlDiagramContext("Data Source=localhost;Initial Catalog=zakon_dev;Integrated Security=True", true, true);
            //var diagramContext = new MsSqlDiagramContext("Data Source=fr;Initial Catalog=zakon_dev;Integrated Security=True");
            diagramContext.GetOperationContext().Load("zakonlibrary.xml");
            //diagramContext.SyncTable("entries");
            //diagramContext.SyncTable("conferences");
            //diagramContext.SyncTable("users_publications");
            //diagramContext.SyncTable("videos");

            var basicGenerator = new BasicGenerator(diagramContext.GetOperationContext(), diagramContext.GetBackendOperationContext());
            basicGenerator.Namespace = "meridian.zakon";
            basicGenerator.RootPath = @"C:\Projects\etcetera\Zakon\trunk\zakon.ru\meridian.zakon";


            Parser parser = new Parser();
            parser.Feeder = new ConsoleFeeder();
            parser.Executor = new SimpleExecutor(diagramContext);
            parser.Generator = basicGenerator;
            parser.Launch();
            diagramContext.GetOperationContext().Save("zakonlibrary.xml");
        }


        static void MasterTourTests()
        {
            //var diagramContext = new MsSqlDiagramContext("Data Source=fr.zakon.etcetera.ws;Initial Catalog=zakon_dev;Integrated Security=False;User Id=sa;Password=futari_1556");
            var diagramContext = new MsSqlDiagramContext("Data Source=localhost;Initial Catalog=avalon2013;Integrated Security=True", false);
            //var diagramContext = new MsSqlDiagramContext("Data Source=fr;Initial Catalog=zakon_dev;Integrated Security=True");
            diagramContext.GetOperationContext().Load("mastertour.xml");

            var basicGenerator = new BasicGenerator(diagramContext.GetOperationContext(), diagramContext.GetBackendOperationContext());
            basicGenerator.Namespace = "meridian.mastertur";
            basicGenerator.RootPath = @"C:\Projects\etcetera\multitour\trunk\meridian.mastertour\";


            Parser parser = new Parser();
            parser.Feeder = new ConsoleFeeder();
            parser.Executor = new SimpleExecutor(diagramContext);
            parser.Generator = basicGenerator;
            parser.Launch();
            diagramContext.GetOperationContext().Save("mastertour.xml");
        }

        static void MultitourTests()
        {
            var dg =
                new MySqlDiagramContext("SERVER=127.0.0.1;DATABASE=multitour2;UID=root;PASSWORD=unix");
            dg.GetOperationContext().Load("multitour.xml");
            dg.SyncAll();

            dg.GetOperationContext().Save("multitour.xml");

            var basicGenerator = new MysqlGenerator(dg.GetOperationContext(), dg.GetBackendOperationContext());
            basicGenerator.Namespace = "meridian.multitour";
            basicGenerator.RootPath = @"C:\Projects\etcetera\multitour\trunk\meridian.multitour\";

            //dg.GetOperationContext().CreateAggregation("proto.reservations.pt_reservations")
            basicGenerator.Generate();
        }

        static void SmolenskTests()
        {
            var dg =
                new MySqlDiagramContext("SERVER=127.0.0.1;DATABASE=smolensk;UID=root;PASSWORD=unix");
            dg.GetOperationContext().Load(@"C:\Projects\etcetera\smolensk\db\smolensk.xml");
            dg.SyncAll();

            dg.GetOperationContext().Save(@"C:\Projects\etcetera\smolensk\db\smolensk.xml");

            var basicGenerator = new MysqlGenerator(dg.GetOperationContext(), dg.GetBackendOperationContext());
            basicGenerator.Namespace = "meridian.smolensk";
            basicGenerator.RootPath = @"C:\Projects\etcetera\smolensk\trunk\meridian.smolensk\";

            //dg.GetOperationContext().CreateAggregation("proto.reservations.pt_reservations")
            basicGenerator.Generate();
        }

        static void FcdynamoTests()
        {
            var dg =
                new MySqlDiagramContext("SERVER=46.4.85.36;DATABASE=fcdynamo_new;UID=videoportal;PASSWORD=portallovesyou");
            dg.GetOperationContext().Load("fcdynamo.xml");
            dg.SyncTable("videos");
            dg.SyncTable("video_cats");
            dg.SyncTable("video_cats_cats");
            dg.SyncTable("video_program");
            dg.SyncTable("video_program_options");
            dg.GetOperationContext().Save("fcdynamo.xml");

            var basicGenerator = new MysqlGenerator(dg.GetOperationContext(), dg.GetBackendOperationContext());
            basicGenerator.Namespace = "meridian.dinamotv";
            basicGenerator.RootPath = @"C:\Projects\etcetera\fcdynamotv\trunk\meridian.dinamotv\";
            basicGenerator.Generate();
        }

        static void IgzakonTests()
        {
            var dg =
                new MySqlDiagramContext("SERVER=127.0.0.1;DATABASE=igzakon;UID=root;PASSWORD=unix");
            dg.SyncTable("zk_articles");
            dg.SyncTable("zk_magazines");
            dg.SyncTable("zk_magazine_types");
            dg.SyncTable("zk_part_types");
            dg.GetOperationContext().Save("igzakon.xml");

            var basicGenerator = new MysqlGenerator(dg.GetOperationContext(), dg.GetBackendOperationContext());
            basicGenerator.Namespace = "meridian.igzakon";
            basicGenerator.RootPath = @"C:\Projects\etcetera\Zakon\trunk\zakon.ru\meridian.igzakon\";
            basicGenerator.Generate();
        }

        static void Main(string[] args)
        {
            
            ZakonTests();
            //MultitourTests();
            //FcdynamoTests();
            //IgzakonTests();
            //MasterTourTests();
            //SmolenskTests();
        }
    }
}
