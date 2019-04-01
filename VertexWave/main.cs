using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using VertexWave.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OculusRiftLib;
using TrueCraft.Client.Rendering;
using VertexWave;
using Voxeland;

class Program
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetDllDirectory(string lpPathName);


    static void Main(string[] args)
    {
        // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            SetDllDirectory(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.Is64BitProcess ? "x64" : "x86"
            ));
        }

        // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
        // NOTE: from documentation: 
        //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
        //           <key>NSHighResolutionCapable</key>
        //           <string>True</string>
        Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

        using (VoxeLand game = new VoxeLand())
        {
            bool isHighDPI = Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1";
            if (isHighDPI)
                Debug.WriteLine("HiDPI Enabled");

            game.Run();
        }
    }
}


class VoxeLand : Game, IGameState
{
    public static List<Node> nodes = new List<Node>();
    GraphicsDeviceManager graphics;

    public static Node root = new Node();

    public static VoxeLand game;
    // new code:
    //AlphaTestEffect effect;
    public static Effect effect;
    //public static Effect tiltShiftEffect;
    Effect alphaEffect;

    BasicEffect basicEffect;


    SpriteBatch spriteBatch;
    //SpriteFont font;
    Texture2D smile;
    Effect exampleEffect;
    private bool isServer;
    private string ip;

    public Camera camera;

    private Vector3 camPosition = new Vector3(0, 7, 0);

    private Texture2D terrain;

    GameTime gameTime;

    FontRenderer font;

    Player player;

    private RenderTarget2D renderTarget { get; set; }
    
    private RenderTarget2D shadowRenderTarget { get; set; }
    
    private RenderTarget2D aoRenderTarget { get; set; }
    
    private RenderTarget2D BeforePostProcessTarget { get; set; }

    private Texture2D shadowMap;
    
    Texture2D ssaoMap;
    
    Texture2D postProcess;


    private long lastTime = 0;
    private long lastCamera = 0;
    double lastTimeFPS = 0;
    
    private static readonly DateTime Start = DateTime.Now;

    RasterizerState counterClockWise = new RasterizerState();

    private OculusRift rift = new OculusRift();

    RenderTarget2D[] renderTargetEye = new RenderTarget2D[2];

    SerialController serialController;

    GameStateListener gameStateListener;

    private Model keyboard;


    //Timer
    private long sinceLoaded = 0;
    private long sinceStarted = 0;
    private long sinceLost = 0;
    private float modeTimer = 1;
    private bool started = false;
    private bool lost = false;

    float modeFactor = 1;

    public static Matrix View;
    public static Matrix Projection;

    public static long CurrentTimeMillis()
    {
        return (long) (DateTime.Now - Start).TotalMilliseconds;
    }

    public VoxeLand()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 800;
        graphics.PreferredBackBufferHeight = 600;
        graphics.GraphicsProfile = GraphicsProfile.HiDef;
        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        Content.RootDirectory = Configuration.Path;

        Window.AllowUserResizing = false;
        IsMouseVisible = false;

        root.Add(new ChunkManager());

        //root.Add(new Mesh());

        game = this;

        //effect = new AlphaTestEffect(GraphicsDevice);//Content.Load<Effect>("diffuse_shader.fbx");

        counterClockWise.CullMode = CullMode.CullClockwiseFace;

        gameStateListener = new GameStateListener();
        gameStateListener.Add(this);
    }

    protected override void Initialize()
    {
        // initialize the Rift
        int result = rift.Init(GraphicsDevice);
        if (result != 0)
            throw new InvalidOperationException("rift.Init result: " + result);

        base.Initialize();
    }


    protected override void LoadContent()
    {
        keyboard = Content.Load<Model>("source/Keyboard");

        basicEffect = new BasicEffect(GraphicsDevice);

        //var conf = File.ReadAllText(Configuration.Path + "conf.json");
        //JObject jConf = JObject.Parse(conf);

        isServer = false;// (bool)jConf["server"];
        ip = "0";// (string)jConf["ip"];

        //root.Add(new Client());
        //Client.SetServerIP(ip);
        /*
        if (isServer)
        {
            root.Add(new Server());
        }
        */

        player = new Player(keyboard,new Vector3(0, WorldGenerator.PathHeight+2, 0));

        root.Add(player);

        serialController = new SerialController(player);

        //camera = new Camera(new Vector3(0, 100, 0), new Vector3(0, 0, 0), Vector3.UnitZ);

        camera = new Camera(player);

        player.SetCamera(camera);

        GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        FileStream filestream = new FileStream(Configuration.Path + "terrain.png", FileMode.Open);
        terrain = Texture2D.FromStream(GraphicsDevice, filestream);


        gameTime = new GameTime();
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // Most content can be loaded from MonoGame Content Builder projects.
        // (Note how "Content.mgcb" has the Build Action "MonoGameContentReference".)
        //font = Content.Load<SpriteFont>("Fonts/Pixel_Regular.fnt");
        //terrain = Content.Load<Texture2D>("terrain.png");

        // Effects need to be loaded from files built by fxc.exe from the DirectX SDK (June 2010)
        // (Note how each .fx file has the Build Action "CompileShader", which produces a .fxb file.)
        //exampleEffect = new Effect(GraphicsDevice, File.ReadAllBytes(@"Effects/ExampleEffect.fxb"));
        //FileStream font = new FileStream("Fonts/Pixel_Regulat.fnt", FileMode.Open);

        font = new FontRenderer(
                new Font(Content, "Fonts/Pixel"),
                new Font(Content, "Fonts/Pixel", FontStyle.Bold), null, null,
                new Font(Content, "Fonts/Pixel", FontStyle.Italic));

        //effect = Content.Load<Effect>("shader.fxc");

        var r = new Random();

        //var  newBear = new NPC("bear", new Vector3(0, 100, 0));
        //root.Add(newBear);

        /*
        for (var i = 0; i < 100; i++)
        {
            newBear = new NPC("bear", new Vector3((float)r.NextDouble() * 20f + i, 100, (float)r.NextDouble() * 20f + i));
            root.Add(newBear);
        }

*/
        
        effect = Content.Load<Effect> ("Shadow");  
        //tiltShiftEffect = Content.Load<Effect> ("TiltShift");

        PresentationParameters pp = GraphicsDevice.PresentationParameters;
        renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        aoRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);
        BeforePostProcessTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        shadowRenderTarget = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24);

        // create one rendertarget for each eye
            for (int eye = 0; eye < 2; eye++) {
                renderTargetEye[eye] = rift.CreateRenderTargetForEye(eye);
            }
    }

    /*
    protected override void Initialize()
    {
        //CreateRenderTarget();
    }
    */

    protected override void UnloadContent()
    {
        Content.Unload();

        //spriteBatch.Dispose();
        //exampleEffect.Dispose();

        base.UnloadContent();
    }



    protected override void Update(GameTime gameTime)
    {
        //Input.Update(IsActive);

        //
        // Asset Rebuilding:
#if DEBUG
        //if(Input.KeyWentDown(Keys.F5))
        {
            //if(AssetRebuild.Run())
            {
                //UnloadContent();
                //LoadContent();
            }
        }

#endif
        //
        // Insert your game update logic here.
        //

        //Console.WriteLine("updating");
        //camPosition += new Vector3(1, 0, 0) * gameTime.ElapsedGameTime.Milliseconds/100f;

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            System.Environment.Exit(0);
        }
        var current  = CurrentTimeMillis();
        var delta = current - lastTime;
        lastTime = current;

        sinceLoaded += delta;
        sinceStarted += delta;
        sinceLost += delta;

        var deltaPercent = delta / 60f;

        serialController.Update();

        root.Update(deltaPercent);

        // update head tracking
        rift.TrackHead();

        if(sinceStarted / 1000 > 30)
        {
            modeTimer -= delta/5000f;
        }

        if(modeTimer > 1)
        {
            modeTimer = 1;
        }

        if (modeTimer < 0)
        {
            modeTimer = 0;
        }

        modeFactor = modeTimer;

        if(!started && sinceLoaded/1000 > 10)
        {
            gameStateListener.StartedGame();
        }

        Console.WriteLine(sinceLost);

        if(lost && sinceLost/1000 > 3)
        {
            Reset();
        }

        base.Update(gameTime);
    }

    private void Reset()
    {
        player.position = new Vector3(0, WorldGenerator.PathHeight + 2, 0);
        started = false;
        lost = false;
        gameStateListener.LoadedGame();
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Enviroment.SkyColor);

        double time = gameTime.ElapsedGameTime.Milliseconds;

        double fps = 1.0 / (time / 1000);

        lastTimeFPS = time;


        //spriteBatch.End();

        float delta = CurrentTimeMillis() - (float)lastCamera;
        lastCamera = CurrentTimeMillis();
        player.Update(delta);
        camera.Update(delta);
        //Thread.Sleep(10);
        for (int eye = 0; eye < 2; eye++)
        {

            GraphicsDevice.RasterizerState = counterClockWise;

            //GraphicsDevice.SetRenderTarget(shadowRenderTarget);
            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            /*
            DrawScene("ShadowMap");
            GraphicsDevice.SetRenderTarget(null);
            shadowMap = (Texture2D)shadowRenderTarget;

            GraphicsDevice.SetRenderTarget(aoRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);


            DrawScene("SSAO",eye);

            GraphicsDevice.SetRenderTarget(null);
            ssaoMap = (Texture2D)aoRenderTarget;

            */

            GraphicsDevice.SetRenderTarget(renderTargetEye[eye]);

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawScene("ShadowedScene",eye);
            //DrawScene("Water",eye);
            shadowMap = null;

            //base.Draw(gameTime);

            postProcess = (Texture2D)BeforePostProcessTarget;

            GraphicsDevice.SetRenderTarget(null);

        }

        //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

        //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
        /*
        Matrix world = Matrix.CreateTranslation(camPosition - new Vector3(0,5,0));
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        */


        //Console.WriteLine(Process.GetCurrentProcess().Threads.Count);

        //font.DrawText(spriteBatch, 0, 0, time.ToString(), 5);

        //GraphicsDevice.SetRenderTarget(null);

        double dist = Math.Sqrt(Math.Pow(player.position.X - camera.position.X, 2) +
                                Math.Pow(player.position.Y - camera.position.Y, 2) +
                                Math.Pow(player.position.Z - camera.position.Z, 2));

        dist /= camera.Distance; //get value between 0 and 1
        
        Console.WriteLine(dist);

        

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
            SamplerState.LinearClamp, DepthStencilState.Default, 
            RasterizerState.CullNone);
        
        //tiltShiftEffect.Parameters["xSSAOMap"].SetValue(ssaoMap);
        //tiltShiftEffect.Parameters["xPlayerDistance"].SetValue((float)dist);
 
        spriteBatch.Draw(postProcess, new Rectangle(0, 0,GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width , GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.White);
 
        spriteBatch.End();
        
        
        
        
        
        spriteBatch.Begin();
        
        string fpsString = ((int)fps).ToString();
        font.DrawText(spriteBatch, 0, 0, fpsString, 5);

        string timeString = Enviroment.Hours.ToString() + ":" + Enviroment.Minutes.ToString();
        font.DrawText(spriteBatch, 0, 150, $"x:{player.position.X}" , 5);
        font.DrawText(spriteBatch, 0, 300, $"z:{player.position.Z}" , 5);
        spriteBatch.End();

        // submit rendertargets to the Rift
        int result = rift.SubmitRenderTargets(renderTargetEye[0], renderTargetEye[1]);

        DrawEyeViewIntoBackbuffer(0);


        //base.Draw(gameTime);

    }

    void DrawEyeViewIntoBackbuffer(int eye)
    {
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        var pp = GraphicsDevice.PresentationParameters;

        int height = pp.BackBufferHeight;
        int width = Math.Min(pp.BackBufferWidth, (int)(height * rift.GetRenderTargetAspectRatio(eye)));
        int offset = (pp.BackBufferWidth - width) / 2;

        spriteBatch.Begin();
        spriteBatch.Draw(renderTargetEye[eye], new Rectangle(offset, 0, width, height), Color.White);
        spriteBatch.End();
    }

    void DrawScene(string technique, int eye = -1)
    {

        var ambientPower = 0.5f;

        var lightPos = new Vector3(0,130,0);

        //lightPos.Y = 300;

        lightPos.X = ((int)player.position.X/8)*8;
        lightPos.Z = ((int)player.position.Z/8)*8;

        var lightPower = 0.5f;
 
        Matrix lightsView = Matrix.CreateLookAt(lightPos, lightPos+new Vector3(0,-1,0), new Vector3(0, 0, 1));
        //Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 0.1f, 100f);
        Matrix lightsProjection = Matrix.CreateOrthographic(800, 800, 1, 130);
 
        var lightsViewProjectionMatrix = lightsView * lightsProjection;
        
        Matrix matrix;
        
        View = Matrix.Identity;
        /*
        if(eye != -1)
        {
            view = camera.GetEye(eye);
        }
        else
        {
            view = camera.view;
        }
        */

        Projection = Matrix.Identity;// =camera.projection;
        if (eye != -1)
        {
            View = rift.GetEyeViewMatrix(eye, Matrix.CreateTranslation(player.position + new Vector3(0,1.5f,0)));
            Projection = rift.GetProjectionMatrix(eye);
        }
        Matrix.Multiply(ref View, ref Projection, out matrix);
        
        effect.CurrentTechnique = effect.Techniques[technique];
        effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * View * Projection);
        effect.Parameters["xCamPosition"].SetValue(player.position + new Vector3(0, 1.5f, 0));
        effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        effect.Parameters["xLightPos"].SetValue(lightPos);
        effect.Parameters["xLightPower"].SetValue(lightPower);
        effect.Parameters["xAmbient"].SetValue(ambientPower);
        effect.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
        effect.Parameters["xFogDistance"].SetValue((float)12 * 16);
        effect.Parameters["xFogGradient"].SetValue((float)2 * 16);
        effect.Parameters["xFogColor"].SetValue(Color.CornflowerBlue.ToVector3());
        effect.Parameters["xModeFactor"].SetValue(modeFactor);

        byte referenceAlpha = 0b11111111;
            
        
        Vector4 alphaTest = new Vector4();
        
        const float threshold = 0.5f / 255f;
        float reference = (float)referenceAlpha / 255f;
        alphaTest.X = reference;
        alphaTest.Y = threshold;
        alphaTest.Z = 1;
        alphaTest.W = -1;
        effect.Parameters["xAlphaTest"].SetValue(alphaTest);


        if (technique == "Water")
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.BlendState = BlendState.Additive;
        }
        else
        {
            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        if (true)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
 
                root.Draw(false);
                root.Draw(true);
            
            }
        }


        /*
        //effect.View = camera.view;
        effect.Parameters["AlphaTest"].SetValue(0b11111111);
        effect.Parameters["Texture"].SetValue(terrain);
        effect.Parameters["WorldViewProj"].SetValue(matrix);
        var light = (float)Enviroment.Brigthness + 0.15f;
        if (light > 1)
        {
            light = 1;
        }
        var ambientLight = (float)Enviroment.Brigthness + 0.2f;
        if (ambientLight > 1)
        {
            ambientLight = 1;
        }
        effect.Parameters["DiffuseColor"].SetValue(new Color(ambientLight, ambientLight, ambientLight).ToVector3());
        effect.Parameters["Light"].SetValue(light);

*/
        //effect.AlphaFunction = CompareFunction.Equal;
        //effect.ReferenceAlpha = 255;

        // effect.VertexColorEnabled = true;
        // effect.Texture = terrain;

        /*
        float aspectRatio =
            graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
        //float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        //float nearClipPlane = 1;
        //float farClipPlane = 500;

        //effect.Projection = camera.projection;
        //effect.Parameters["Projection"].SetValue(camera.projection);
        Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        // effect.Parameters["WorldInverseTranspose"].SetValue(world);

        //effect.EnableDefaultLighting();
        // effect.FogEnabled = true;
        // effect.FogColor = Enviroment.SkyColor.ToVector3();
        // effect.FogStart = (ChunkManager.MaxChunkDistance - 1) * WorldGenerator.ChunkSize;
        // effect.FogEnd = ChunkManager.MaxChunkDistance * WorldGenerator.ChunkSize + WorldGenerator.ChunkSize / 4;
        //effect.EmissiveColor = new Vector3(1, 0, 0);


        GraphicsDevice.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
        GraphicsDevice.BlendState = BlendState.Opaque;

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            root.Draw(false);
           
        }

        GraphicsDevice.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
        GraphicsDevice.BlendState = BlendState.Opaque;

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            root.Draw(true);

        }
        
        */
    }

    public void LostGame()
    {
        if(lost == false)
        {
            sinceLost = 0;
            lost = true;
        }
    }

    public void StartedGame()
    {
        sinceStarted = 0;
        started = true;
    }

    public void LoadedGame()
    {
        sinceLoaded = 0;
        sinceStarted = 0;
        lost = false;
        started = false;
        modeFactor = 1;
        modeTimer = 1;
    }
}
