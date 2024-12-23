using PlushIT.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlushIT.Utilities;
using HelixToolkit.Wpf;

namespace PlushIT.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel MainViewModel { get; } = new();

        private bool isLeftDown = false;
        private bool isRightDown = false;
        private double startingLeftX;
        private double startingLeftY;
        private double startingLeftZ;
        private double startingRightX;
        private double startingRightY;
        private double startingRightZ;
        private double zoom = .1d;

        public MainWindow()
        {
            DataContext = MainViewModel;
            InitializeComponent();
        }

        private void HelixViewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition((IInputElement)sender);

            List<RayMeshGeometry3DHitTestResult> s = CastRay(point, (HelixViewport3D)sender);
        }

        private void HelixViewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton != MouseButtonState.Pressed)
            {
                MainViewModel.LinesGroup.Children.Clear();

                Point point = e.GetPosition((IInputElement)sender);

                RayMeshGeometry3DHitTestResult? s = CastRay(point, (HelixViewport3D)sender).FirstOrDefault();

                if (s is not null)
                {
                    MainViewModel.HighlightVertexFromHitTest(s);
                    //Point3D pos1 = s.MeshHit.Positions[s.VertexIndex1];
                    //Point3D pos2 = s.MeshHit.Positions[s.VertexIndex2];
                    //Point3D pos3 = s.MeshHit.Positions[s.VertexIndex3];

                    //MeshGeometry3D hoverMesh = new();
                    //hoverMesh.Positions.Add(pos1);
                    //hoverMesh.Positions.Add(pos2);
                    //hoverMesh.Positions.Add(pos3);
                    //hoverMesh.TriangleIndices.Add(0);
                    //hoverMesh.TriangleIndices.Add(1);
                    //hoverMesh.TriangleIndices.Add(2);

                    //GeometryModel3D hoverModel = new(hoverMesh, new DiffuseMaterial(Brushes.Yellow));
                    //MainViewModel.LinesGroup.Children.Add(hoverModel);
                }
            }
        }

        public static List<RayMeshGeometry3DHitTestResult> CastRay<T>(Point clickPoint, HelixViewport3D viewPort, IEnumerable<Visual3D>? ignoreVisuals = null)
        {
            List<RayMeshGeometry3DHitTestResult> retVal = [];

            //  This gets called every time there is a hit
            HitTestResultBehavior resultCallback(HitTestResult result)
            {
                if (result is RayMeshGeometry3DHitTestResult resultCast)       //  It could also be a RayHitTestResult, which isn't as exact as RayMeshGeometry3DHitTestResult
                {
                    if ((ignoreVisuals == null || !ignoreVisuals.Any(o => o == resultCast.VisualHit)) && resultCast.VisualHit is T)
                    {
                        retVal.Add(resultCast);
                    }
                }

                return HitTestResultBehavior.Continue;
            }

            //  Get hits against existing models
            VisualTreeHelper.HitTest(viewPort, null, resultCallback, new PointHitTestParameters(clickPoint));

            //  Exit Function
            return retVal;
        }

        public static List<RayMeshGeometry3DHitTestResult> CastRay(Point clickPoint, HelixViewport3D viewPort, IEnumerable<Visual3D>? ignoreVisuals = null)
        {
            List<RayMeshGeometry3DHitTestResult> retVal = [];

            //  This gets called every time there is a hit
            HitTestResultBehavior resultCallback(HitTestResult result)
            {
                if (result is RayMeshGeometry3DHitTestResult resultCast)       //  It could also be a RayHitTestResult, which isn't as exact as RayMeshGeometry3DHitTestResult
                {
                    if (ignoreVisuals == null || !ignoreVisuals.Any(o => o == resultCast.VisualHit))
                    {
                        retVal.Add(resultCast);
                    }
                }

                return HitTestResultBehavior.Continue;
            }

            //  Get hits against existing models
            VisualTreeHelper.HitTest(viewPort, null, resultCallback, new PointHitTestParameters(clickPoint));

            //  Exit Function
            return retVal;
        }


        //private void Window_PreviewKeyDown(object sender, KeyEventArgs e) => camera.MoveBy(e.Key).RotateBy(e.Key);

        //Point from;
        //private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    var till = e.GetPosition(sender as IInputElement);
        //    double dx = till.X - from.X;
        //    double dy = till.Y - from.Y;
        //    from = till;

        //    var distance = dx * dx + dy * dy;
        //    if (distance <= 0d)
        //        return;

        //    if (e.MouseDevice.LeftButton is MouseButtonState.Pressed)
        //    {
        //        var angle = (distance / camera.FieldOfView) % 45d;
        //        camera.Rotate(new(dy, -dx, 0d), angle);
        //    }
        //}
    }
}