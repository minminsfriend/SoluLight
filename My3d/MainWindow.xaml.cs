using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace My3d
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // 카메라 제어 변수
        private PerspectiveCamera _camera;
        private double _anglePhi = 0;   // 좌우 회전 각도
        private double _angleTheta = Math.PI / 2; // 상하 회전 각도
        private double _radius = 5;     // 카메라 거리
        private Point _lastMousePos;    // 마지막 마우스 위치

        public MainWindow()
        {
            InitializeComponent();
            SetupScene();
        }

        private void SetupScene()
        {
            // 1. 카메라 초기화
            _camera = new PerspectiveCamera { FieldOfView = 45 };
            UpdateCameraPosition();
            mainViewport.Camera = _camera;

            // 2. 삼각형 생성 (중심 0,0,0 근처)
            MeshGeometry3D mesh = new MeshGeometry3D();

            var filemesh = @"D:\Works\vs\SoluLight\MyPy\blender\meshdata.txt";

            LoadMeshFromFile(mesh, filemesh);

            ///* 베텍스 좌표 더하기 */
            //mesh.Positions.Add(new Point3D(-1, -1, 0));
            //mesh.Positions.Add(new Point3D(1, -1, 0));
            //mesh.Positions.Add(new Point3D(0, 1, 0));

            ///* 각 페이스에 해당하는 베텍스 인덱스 더하기 */
            //mesh.TriangleIndices.Add(0); 
            //mesh.TriangleIndices.Add(1); 
            //mesh.TriangleIndices.Add(2);

            GeometryModel3D model = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.DeepSkyBlue));
            model.BackMaterial = new DiffuseMaterial(Brushes.IndianRed); // 뒷면 색상


            // 5. 조명 추가 및 시각화 (기존 코드 유지)
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            group.Children.Add(new AmbientLight(Color.FromRgb(50, 50, 50)));
            group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, -1, -3)));

            mainViewport.Children.Add(new ModelVisual3D { Content = group });

            // 와이어프레임과 정점을 추가하려면 AddWireframe, AddVertices 호출 (선택 사항)
            // AddWireframe(group); 
            // AddVertices(group);
        }
        void LoadMeshFromFile(MeshGeometry3D mesh, string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);

            bool readingVertices = false;
            bool readingFaces = false;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                // Check section headers
                if (trimmed == "# Vertices")
                {
                    readingVertices = true;
                    readingFaces = false;
                    continue;
                }
                else if (trimmed == "# Faces")
                {
                    readingVertices = false;
                    readingFaces = true;
                    continue;
                }

                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                // Parse vertices
                if (readingVertices && trimmed.Contains(":"))
                {
                    string[] parts = trimmed.Split(':');
                    if (parts.Length == 2)
                    {
                        string[] coords = parts[1].Split(',');
                        if (coords.Length == 3)
                        {
                            double x = double.Parse(coords[0].Trim());
                            double y = double.Parse(coords[1].Trim());
                            double z = double.Parse(coords[2].Trim());

                            /* 버텍스 좌표 더하기 */
                            mesh.Positions.Add(new Point3D(x, y, z));
                        }
                    }
                }

                // Parse faces
                if (readingFaces && trimmed.Contains(","))
                {
                    string[] indices = trimmed.Split(',');
                    if (indices.Length == 3)
                    {
                        int v1 = int.Parse(indices[0].Trim());
                        int v2 = int.Parse(indices[1].Trim());
                        int v3 = int.Parse(indices[2].Trim());

                        /* 각 페이스에 해당하는 버텍스 인덱스 더하기 */
                        mesh.TriangleIndices.Add(v1);
                        mesh.TriangleIndices.Add(v2);
                        mesh.TriangleIndices.Add(v3);
                    }
                }
            }
        }

        private void SetupScene00()
        {
            // 1. 카메라 초기화
            _camera = new PerspectiveCamera { FieldOfView = 45 };
            UpdateCameraPosition();
            mainViewport.Camera = _camera;

            // 2. 삼각형 생성 (중심 0,0,0 근처)
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(-1, -1, 0));
            mesh.Positions.Add(new Point3D(1, -1, 0));
            mesh.Positions.Add(new Point3D(0, 1, 0));
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2);

            //GeometryModel3D model = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.DeepSkyBlue));
            //model.BackMaterial = new DiffuseMaterial(Brushes.IndianRed); // 뒷면 색상

            // !! 핵심: 텍스처 좌표(UV 좌표) 추가 !!
            // Positions의 각 점에 대응하는 이미지 좌표 (0,0) ~ (1,1) 사이 값
            mesh.TextureCoordinates.Add(new Point(0, 1)); // 점 0 (-1,-1,0) -> 이미지 좌하단
            mesh.TextureCoordinates.Add(new Point(1, 1)); // 점 1 (1,-1,0)  -> 이미지 우하단
            mesh.TextureCoordinates.Add(new Point(0.5, 0)); // 점 2 (0,1,0)  -> 이미지 중앙 상단

            // 3. 재질 생성: 이미지 브러시 사용
            var fileimage = @"c:\Users\dgtlo\Downloads\참조이미지\정크선2.jpg"; // <-- 여기에 실제 이미지 경로 입력

            // BitmapImage를 사용하여 이미지 로드
            BitmapImage bitmap = new BitmapImage(new Uri(fileimage, UriKind.RelativeOrAbsolute));
            ImageBrush imageBrush = new ImageBrush(bitmap);

            // DiffuseMaterial에 ImageBrush 할당
            DiffuseMaterial material = new DiffuseMaterial(imageBrush);

            // 4. 모델 생성 (재질 + 기하구조)
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = mesh;
            model.Material = material;
            model.BackMaterial = new DiffuseMaterial(Brushes.Gray); // 뒷면 재질 (선택 사항)

            // 5. 조명 추가 및 시각화 (기존 코드 유지)
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            group.Children.Add(new AmbientLight(Color.FromRgb(50, 50, 50)));
            group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, -1, -3)));

            mainViewport.Children.Add(new ModelVisual3D { Content = group });

            // 와이어프레임과 정점을 추가하려면 AddWireframe, AddVertices 호출 (선택 사항)
            // AddWireframe(group); 
            // AddVertices(group);
        }

        // 카메라 위치 업데이트 (구면 좌표 -> 직교 좌표 변환)
        private void UpdateCameraPosition()
        {
            double x = _radius * Math.Sin(_angleTheta) * Math.Sin(_anglePhi);
            double y = _radius * Math.Cos(_angleTheta);
            double z = _radius * Math.Sin(_angleTheta) * Math.Cos(_anglePhi);

            _camera.Position = new Point3D(x, y, z);
            _camera.LookDirection = new Vector3D(-x, -y, -z); // 항상 원점을 바라봄
        }

        // --- 마우스 이벤트 핸들러 ---

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePos = e.GetPosition(mainViewport);
            mainViewport.CaptureMouse(); // 마우스 포커스 고정
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mainViewport.ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point currentPos = e.GetPosition(mainViewport);
                double deltaX = currentPos.X - _lastMousePos.X;
                double deltaY = currentPos.Y - _lastMousePos.Y;

                _anglePhi -= deltaX * 0.01;   // 감도 조절
                _angleTheta -= deltaY * 0.01;

                // 상하 회전 제한 (짐벌락 방지)
                _angleTheta = Math.Max(0.1, Math.Min(Math.PI - 0.1, _angleTheta));

                UpdateCameraPosition();
                _lastMousePos = currentPos;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _radius -= e.Delta * 0.005; // 줌 인/아웃
            _radius = Math.Max(2, Math.Min(20, _radius)); // 거리 제한
            UpdateCameraPosition();
        }
        // SetupScene() 메서드 안에서 호출하세요
        private void AddWireframe(Model3DGroup group)
        {
            // 삼각형의 세 꼭짓점 (기존 삼각형 좌표와 동일)
            Point3D p0 = new Point3D(-1, -1, 0);
            Point3D p1 = new Point3D(1, -1, 0);
            Point3D p2 = new Point3D(0, 1, 0);

            // 각 변을 잇는 선분 추가
            group.Children.Add(CreateLine(p0, p1, Colors.Red));
            group.Children.Add(CreateLine(p1, p2, Colors.Red));
            group.Children.Add(CreateLine(p2, p0, Colors.Red));
        }
        private void AddVertices(Model3DGroup group)
        {
            // 삼각형의 세 꼭짓점 좌표
            Point3D[] points = {
                new Point3D(-1, -1, 0),
                new Point3D(1, -1, 0),
                new Point3D(0, 1, 0)
            };

            foreach (var pt in points)
            {
                group.Children.Add(CreatePoint(pt, 0.05, Colors.Blue)); // 크기 0.05의 청색 점
            }
        }
        private Model3DGroup CreatePoint(Point3D center, double size, Color color)
        {
            Model3DGroup pointGroup = new Model3DGroup();
            MeshGeometry3D cubeMesh = new MeshGeometry3D();
            double s = size / 2;

            // 정육면체(점)의 8개 정점 정의
            cubeMesh.Positions = new Point3DCollection {
                new Point3D(center.X-s, center.Y-s, center.Z+s), new Point3D(center.X+s, center.Y-s, center.Z+s),
                new Point3D(center.X+s, center.Y+s, center.Z+s), new Point3D(center.X-s, center.Y+s, center.Z+s),
                new Point3D(center.X-s, center.Y-s, center.Z-s), new Point3D(center.X+s, center.Y-s, center.Z-s),
                new Point3D(center.X+s, center.Y+s, center.Z-s), new Point3D(center.X-s, center.Y+s, center.Z-s)
            };

            // 정육면체 면 구성 (삼각형 인덱스)
            cubeMesh.TriangleIndices = new Int32Collection {
                0,1,2, 0,2,3, 5,4,7, 5,7,6, 1,5,6, 1,6,2, // 앞, 뒤, 우
                4,0,3, 4,3,7, 3,2,6, 3,6,7, 1,0,4, 1,4,5  // 좌, 상, 하
            };

            // 조명에 관계없이 잘 보이도록 EmissiveMaterial 사용
            MaterialGroup mat = new MaterialGroup();
            mat.Children.Add(new DiffuseMaterial(new SolidColorBrush(color)));
            mat.Children.Add(new EmissiveMaterial(new SolidColorBrush(color)));

            pointGroup.Children.Add(new GeometryModel3D(cubeMesh, mat));
            return pointGroup;
        }
        private Model3DGroup CreateLine(Point3D start, Point3D end, Color color)
        {
            Model3DGroup lineGroup = new Model3DGroup();
            double thickness = 0.02; // 선의 두께

            // 선을 그리기 위한 메쉬 생성 (매우 얇은 사각형/삼각형 조합)
            MeshGeometry3D lineMesh = new MeshGeometry3D();

            // 두 점 사이의 방향 벡터 계산
            Vector3D delta = end - start;

            // 선에 수직인 벡터를 구하여 두께를 만듦
            Vector3D side = Vector3D.CrossProduct(delta, new Vector3D(0, 0, 1));
            if (side.Length < 0.01) side = Vector3D.CrossProduct(delta, new Vector3D(0, 1, 0));
            side.Normalize();
            side *= thickness;

            // 선을 구성하는 4개의 점 (얇은 면)
            lineMesh.Positions.Add(start - side);
            lineMesh.Positions.Add(start + side);
            lineMesh.Positions.Add(end + side);
            lineMesh.Positions.Add(end - side);

            lineMesh.TriangleIndices.Add(0); lineMesh.TriangleIndices.Add(1); lineMesh.TriangleIndices.Add(2);
            lineMesh.TriangleIndices.Add(0); lineMesh.TriangleIndices.Add(2); lineMesh.TriangleIndices.Add(3);

            // 빛의 영향을 받지 않는 붉은색 방사 재질(EmissiveMaterial) 사용
            GeometryModel3D lineModel = new GeometryModel3D(lineMesh, new EmissiveMaterial(new SolidColorBrush(color)));
            lineGroup.Children.Add(lineModel);

            return lineGroup;
        }
    }
}
