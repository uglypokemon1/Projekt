using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using Szeminarium;

namespace GrafikaSzeminarium
{
    internal class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static ModelObjectDescriptor[] cube = new ModelObjectDescriptor[27];

        private static CameraDescriptor camera = new CameraDescriptor();


        private const string ModelMatrixVariableName = "uModel";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = uProjection*uView*uModel*vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        private static uint program;

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Grafika szeminárium";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;
            graphicWindow.Closing += GraphicWindow_Closing;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Closing()
        {
            Gl.DeleteProgram(program);
        }

        private static void GraphicWindow_Load()
        {
            Gl = graphicWindow.CreateOpenGL();
            float[] cyan = { 0.0f, 1.0f, 1.0f, 1.0f };
            float[] yellow = { 1.0f, 0.835f, 0.0f, 1.0f };
            float[] blue = { 0.0f, 0.274f, 0.678f, 1.0f };
            float[] green = { 0.0f, 0.608f, 0.282f, 1.0f };
            float[] red = { 0.717f, 0.070f, 0.203f, 1.0f };
            float[] orange = { 1.0f, 0.345f, 0.0f, 1.0f };
            float[] black = { 0.0f, 0.0f, 0.0f, 1.0f };
            CreateCubes(cyan, yellow, blue, green, red, orange, black);

            Gl.ClearColor(System.Drawing.Color.White);

            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);


            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fshader));

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);

            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
            if ((ErrorCode)Gl.GetError() != ErrorCode.NoError)
            {

            }

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
        }

        private static void CreateCubes(float[] cyan, float[] yellow, float[] blue, float[] green, float[] red, float[] orange, float[] black)
        {
            cube[0] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, cyan, black, black, black, black);
            cube[1] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, cyan, black, red, black, black);
            cube[2] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, cyan, black, red, black, blue);
            cube[3] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, cyan, green, red, black, black);
            cube[4] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, cyan, green, black, black, black);
            cube[5] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, cyan, green, black, black, black);
            cube[6] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, cyan, black, black, black, black);
            cube[7] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, cyan, black, black, black, blue);
            cube[8] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, cyan, black, black, black, blue);
            // 1 oldal front cyan szinu

            cube[9] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, black, black, black, blue);
            cube[10] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, black, black, yellow, blue);
            cube[11] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, black, black, yellow, blue);
            cube[12] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, black, red, yellow, blue);
            cube[13] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, black, red, black, blue);
            cube[14] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, black, black, black, blue);
            // 2 old right blue

            cube[15] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, green, black, black, black);
            cube[16] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, green, black, yellow, black);
            cube[17] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, black, black, yellow, black);
            cube[18] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, black, black, black, black);
            // 3 old top orange

            cube[19] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, green, black, black, black);
            cube[20] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, green, red, black, black);
            cube[21] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, green, red, yellow, black);
            cube[22] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, green, black, yellow, black);
            // 4 old left green

            cube[23] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, black, red, black, black);
            cube[24] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, black, black, black, red, yellow, black);
            // 5 old bottom red

            cube[25] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, black, black, yellow, black);
            // 6 old back yellow

            cube[26] = ModelObjectDescriptor.CreateCubeWithFaceColors(Gl, orange, black, black, black, black, black);
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO OpenGL
            // make it threadsafe
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);

            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(projectionMatrix, ProjectionMatrixVariableName);


            var modelMatrixCenterCube = Matrix4X4<float>.Identity;
            SetMatrix(modelMatrixCenterCube, ModelMatrixVariableName);
            DrawModelObject(cube2);

            Matrix4X4<float> diamondScale = Matrix4X4.CreateScale(0.25f);
            Matrix4X4<float> rotx = Matrix4X4.CreateRotationX((float)Math.PI / 4f);
            Matrix4X4<float> rotz = Matrix4X4.CreateRotationZ((float)Math.PI / 4f);
            Matrix4X4<float> roty = Matrix4X4.CreateRotationY((float)Math.PI / 2f);
            Matrix4X4<float> trans = Matrix4X4.CreateTranslation(1f, 1f, 0f);
            Matrix4X4<float> dimondCubeModelMatrix = diamondScale * rotx * rotz * roty * trans;
            SetMatrix(dimondCubeModelMatrix, ModelMatrixVariableName);
            DrawModelObject(cube2);

        }

        private static unsafe void DrawModelObject(ModelObjectDescriptor modelObject)
        {
            Gl.BindVertexArray(modelObject.Vao);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, modelObject.Indices);
            Gl.DrawElements(PrimitiveType.Triangles, modelObject.IndexArrayLength, DrawElementsType.UnsignedInt, null);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        private static unsafe void SetMatrix(Matrix4X4<float> mx, string uniformName)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&mx);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}