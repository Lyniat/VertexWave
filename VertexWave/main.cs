using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OculusRiftLib;
using TrueCraft.Client.Rendering;
using VertexWave;
using VertexWave.Interfaces;

internal class Program
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);


    private static void Main(string[] args)
    {
        // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            SetDllDirectory(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.Is64BitProcess ? "x64" : "x86"
            ));

        // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
        // NOTE: from documentation: 
        //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
        //           <key>NSHighResolutionCapable</key>
        //           <string>True</string>
        Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

        using (var game = new VoxeLand())
        {
            var isHighDpi = Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1";
            if (isHighDpi)
                Debug.WriteLine("HiDPI Enabled");

            game.Run();
        }
    }
}


internal class VoxeLand : Game, IGameState
{
    public static List<Node> nodes = new List<Node>();

    public static Node root = new Node();

    public static VoxeLand game;

    // new code:
    //AlphaTestEffect effect;
    public static Effect effect;

    private static readonly DateTime Start = DateTime.Now;

    public static Matrix View;

    public static Matrix Projection;

    private readonly RasterizerState _counterClockWise = new RasterizerState();

    private readonly GameStateListener _gameStateListener;
    private readonly GraphicsDeviceManager _graphics;

    private readonly RenderTarget2D[] _renderTargetEye = new RenderTarget2D[2];

    private readonly OculusRift _rift = new OculusRift();

    //public static Effect tiltShiftEffect;
    private Effect _alphaEffect;

    private BasicEffect _basicEffect;

    private Vector3 _camPosition = new Vector3(0, 7, 0);
    private Effect _exampleEffect;

    private FontRenderer _font;

    private GameTime _gameTime;
    private string _ip;
    private bool _isServer;

    private Model _keyboard;
    private long _lastCamera;


    private long _lastTime;
    private double _lastTimeFps;
    private Model _logo;

    private Logo _logoObject;
    private bool _lost;

    private float _modeFactor = 1;
    private float _modeTimer = 1;

    private List<Song> _musicTracks;

    private Player _player;

    private Texture2D _postProcess;

    private Random _random;

    private SerialController _serialController;

    private Texture2D _shadowMap;


    //Timer
    private long _sinceLoaded;
    private long _sinceLost;

    private long _sinceStarted;

    //SpriteFont font;
    private Texture2D _smile;


    private SpriteBatch _spriteBatch;

    private Texture2D _ssaoMap;
    private bool _started;

    private float _startModeFactor = 1;

    public Camera camera;

    protected override void OnExiting(Object sender, EventArgs args)
    {
        Environment.Exit(Environment.ExitCode);
        base.OnExiting(sender, args);

    }

    public VoxeLand()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.IsFullScreen = false;
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        _graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        Content.RootDirectory = Configuration.Path;

        Window.AllowUserResizing = false;
        IsMouseVisible = false;

        root.Add(new ChunkManager());

        //root.Add(new Mesh());

        game = this;

        //effect = new AlphaTestEffect(GraphicsDevice);//Content.Load<Effect>("diffuse_shader.fbx");

        _counterClockWise.CullMode = CullMode.CullClockwiseFace;

        _gameStateListener = new GameStateListener();
        _gameStateListener.Add(this);
    }

    private RenderTarget2D RenderTarget { get; set; }

    private RenderTarget2D ShadowRenderTarget { get; set; }

    private RenderTarget2D AoRenderTarget { get; set; }

    private RenderTarget2D BeforePostProcessTarget { get; set; }

    public void LostGame()
    {
        if (_lost == false)
        {
            _sinceLost = 0;
            _lost = true;
        }
    }

    public void StartedGame()
    {
        _sinceStarted = 0;
        _started = true;

        var trackNum = _random.Next(0, _musicTracks.Count);

        MediaPlayer.Play(_musicTracks[trackNum]);
        MediaPlayer.Volume = 1;
    }

    public void LoadedGame()
    {
        MediaPlayer.Stop();
        _sinceLoaded = 0;
        _sinceStarted = 0;
        _lost = false;
        _started = false;
        _modeFactor = 1;
        _modeTimer = 1;
    }

    public static long CurrentTimeMillis()
    {
        return (long) (DateTime.Now - Start).TotalMilliseconds;
    }

    protected override void Initialize()
    {
        // initialize the Rift
        var result = _rift.Init(GraphicsDevice);
        if (result != 0)
            throw new InvalidOperationException("rift.Init result: " + result);

        base.Initialize();
    }


    protected override void LoadContent()
    {
        _random = new Random(DateTime.Now.Millisecond);
        _keyboard = Content.Load<Model>("keyboard");
        _logo = Content.Load<Model>("logo");

        _basicEffect = new BasicEffect(GraphicsDevice);

        //var conf = File.ReadAllText(Configuration.Path + "conf.json");
        //JObject jConf = JObject.Parse(conf);

        _isServer = false; // (bool)jConf["server"];
        _ip = "0"; // (string)jConf["ip"];

        //root.Add(new Client());
        //Client.SetServerIP(ip);
        /*
        if (isServer)
        {
            root.Add(new Server());
        }
        */

        _player = new Player(_keyboard, new Vector3(0, WorldGenerator.PathHeight + 2, 0));

        _logoObject = new Logo(_logo, new Vector3(0, WorldGenerator.PathHeight + 20, 0));

        root.Add(_player);

        root.Add(_logoObject);

        _serialController = new SerialController(_player);

        //camera = new Camera(new Vector3(0, 100, 0), new Vector3(0, 0, 0), Vector3.UnitZ);

        camera = new Camera(_player);

        _player.SetCamera(camera);

        GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;


        _gameTime = new GameTime();
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Most content can be loaded from MonoGame Content Builder projects.
        // (Note how "Content.mgcb" has the Build Action "MonoGameContentReference".)
        //font = Content.Load<SpriteFont>("Fonts/Pixel_Regular.fnt");
        //terrain = Content.Load<Texture2D>("terrain.png");

        // Effects need to be loaded from files built by fxc.exe from the DirectX SDK (June 2010)
        // (Note how each .fx file has the Build Action "CompileShader", which produces a .fxb file.)
        //exampleEffect = new Effect(GraphicsDevice, File.ReadAllBytes(@"Effects/ExampleEffect.fxb"));
        //FileStream font = new FileStream("Fonts/Pixel_Regulat.fnt", FileMode.Open);

        _font = new FontRenderer(
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

        effect = Content.Load<Effect>("Shader");

        var pp = GraphicsDevice.PresentationParameters;
        RenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true,
            GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        AoRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
            SurfaceFormat.Single, DepthFormat.Depth24);
        BeforePostProcessTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true,
            GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        ShadowRenderTarget =
            new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24);

        // create one rendertarget for each eye
        for (var eye = 0; eye < 2; eye++) _renderTargetEye[eye] = _rift.CreateRenderTargetForEye(eye);

        LoadMusic();
    }

    private void LoadMusic()
    {
        _musicTracks = new List<Song>();
        _musicTracks.Add(Content.Load<Song>("music/activation"));
        _musicTracks.Add(Content.Load<Song>("music/escape-from-reality"));
        _musicTracks.Add(Content.Load<Song>("music/impact"));
        _musicTracks.Add(Content.Load<Song>("music/overtake"));
        _musicTracks.Add(Content.Load<Song>("music/sequential-movement"));
        _musicTracks.Add(Content.Load<Song>("music/street-traffic"));
        _musicTracks.Add(Content.Load<Song>("music/turismo"));
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
        OnInput();
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
            Environment.Exit(0);
        }

        var current = CurrentTimeMillis();
        var delta = current - _lastTime;
        _lastTime = current;

        _sinceLoaded += delta;
        _sinceStarted += delta;
        _sinceLost += delta;

        var deltaPercent = delta / 60f;

        _serialController.Update();

        root.Update(deltaPercent);

        // update head tracking
        _rift.TrackHead();

        if (_sinceStarted / 1000 > 30) _modeTimer -= delta / 5000f;

        if (_modeTimer > 1) _modeTimer = 1;

        if (_modeTimer < 0) _modeTimer = 0;

        _modeFactor = _modeTimer;

        if (!_started && _sinceLoaded / 1000 > 10) _gameStateListener.StartedGame();

        Console.WriteLine(_sinceLost);

        if (_lost) MediaPlayer.Volume = 1 - _sinceLost / 3000f;

        if (_lost && _sinceLost / 1000 > 3) Reset();

        _startModeFactor = 1 + _player.position.Z / 80f;

        if (_startModeFactor < 0) _startModeFactor = 0;

        base.Update(gameTime);
    }

    private void OnInput()
    {
        KeyboardState state = Keyboard.GetState();
        var isInput = false;
        if (state.IsKeyDown(Keys.Right))
        {
            _player.OnInput(-15);
            isInput = true;
        }

        if (state.IsKeyDown(Keys.Left))
        {
            _player.OnInput(15);
            isInput = true;
        }

        if (!isInput)
        {
            _player.OnInput(0);
        }
    }

    private void Reset()
    {
        _player.position = new Vector3(0, WorldGenerator.PathHeight + 2, 0);
        _started = false;
        _lost = false;
        _gameStateListener.LoadedGame();
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Enviroment.SkyColor);

        double time = gameTime.ElapsedGameTime.Milliseconds;

        var fps = 1.0 / (time / 1000);

        _lastTimeFps = time;


        //spriteBatch.End();

        var delta = CurrentTimeMillis() - (float) _lastCamera;
        _lastCamera = CurrentTimeMillis();
        _player.Update(delta);
        camera.Update(delta);
        //Thread.Sleep(10);
        for (var eye = 0; eye < 2; eye++)
        {
            GraphicsDevice.RasterizerState = _counterClockWise;

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

            GraphicsDevice.SetRenderTarget(_renderTargetEye[eye]);

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawScene("ShadowedScene", eye);
            //DrawScene("Water",eye);
            _shadowMap = null;

            //base.Draw(gameTime);

            _postProcess = BeforePostProcessTarget;

            GraphicsDevice.SetRenderTarget(null);

            //For display
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawScene("ShadowedScene", eye);
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

        var dist = Math.Sqrt(Math.Pow(_player.position.X - camera.position.X, 2) +
                             Math.Pow(_player.position.Y - camera.position.Y, 2) +
                             Math.Pow(_player.position.Z - camera.position.Z, 2));

        dist /= camera.Distance; //get value between 0 and 1

        Console.WriteLine(dist);


        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone);

        //tiltShiftEffect.Parameters["xSSAOMap"].SetValue(ssaoMap);
        //tiltShiftEffect.Parameters["xPlayerDistance"].SetValue((float)dist);

        _spriteBatch.Draw(_postProcess,
            new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.White);

        _spriteBatch.End();


        _spriteBatch.Begin();

        var fpsString = ((int) fps).ToString();
        _font.DrawText(_spriteBatch, 0, 0, fpsString, 5);

        var timeString = Enviroment.Hours + ":" + Enviroment.Minutes;
        _font.DrawText(_spriteBatch, 0, 150, $"x:{_player.position.X}", 5);
        _font.DrawText(_spriteBatch, 0, 300, $"z:{_player.position.Z}", 5);
        _spriteBatch.End();

        // submit rendertargets to the Rift
        var result = _rift.SubmitRenderTargets(_renderTargetEye[0], _renderTargetEye[1]);

        DrawEyeViewIntoBackbuffer(0);


        //base.Draw(gameTime);
    }

    private void DrawEyeViewIntoBackbuffer(int eye)
    {
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        var pp = GraphicsDevice.PresentationParameters;

        var height = pp.BackBufferHeight;
        var width = Math.Min(pp.BackBufferWidth, (int) (height * _rift.GetRenderTargetAspectRatio(eye)));
        var offset = (pp.BackBufferWidth - width) / 2;

        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTargetEye[eye], new Rectangle(offset, 0, width, height), Color.White);
        _spriteBatch.End();
    }

    private void DrawScene(string technique, int eye = -1)
    {
        var ambientPower = 0.5f;

        var lightPos = new Vector3(0, 130, 0);

        //lightPos.Y = 300;

        lightPos.X = (int) _player.position.X / 8 * 8;
        lightPos.Z = (int) _player.position.Z / 8 * 8;

        var lightPower = 0.5f;

        var lightsView = Matrix.CreateLookAt(lightPos, lightPos + new Vector3(0, -1, 0), new Vector3(0, 0, 1));
        //Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 0.1f, 100f);
        var lightsProjection = Matrix.CreateOrthographic(800, 800, 1, 130);

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

        Projection = Matrix.Identity; // =camera.projection;
        if (eye != -1)
        {
            View = _rift.GetEyeViewMatrix(eye, Matrix.CreateTranslation(_player.position + new Vector3(0, 1.5f, 0)));
            Projection = _rift.GetProjectionMatrix(eye);
        }

        Matrix.Multiply(ref View, ref Projection, out matrix);

        effect.CurrentTechnique = effect.Techniques[technique];
        effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * View * Projection);
        effect.Parameters["xCamPosition"].SetValue(_player.position + new Vector3(0, 1.5f, 0));
        effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        effect.Parameters["xLightPos"].SetValue(lightPos);
        effect.Parameters["xLightPower"].SetValue(lightPower);
        effect.Parameters["xAmbient"].SetValue(ambientPower);
        effect.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
        effect.Parameters["xFogDistance"].SetValue((float) 12 * 16);
        effect.Parameters["xFogGradient"].SetValue((float) 2 * 16);
        effect.Parameters["xFogColor"].SetValue(Color.CornflowerBlue.ToVector3());
        effect.Parameters["xModeFactor"].SetValue(_modeFactor);
        effect.Parameters["xStartModeFactor"].SetValue(_startModeFactor);

        byte referenceAlpha = 0b11111111;


        var alphaTest = new Vector4();

        const float threshold = 0.5f / 255f;
        var reference = referenceAlpha / 255f;
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
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                root.Draw(false);
                root.Draw(true);
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
}