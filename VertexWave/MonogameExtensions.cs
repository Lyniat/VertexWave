using System;
using System.Reflection;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class MonoGameExtensions
    {
        public static void GetNativeDxDeviceAndContext(this GraphicsDevice graphicsDevice, out IntPtr dxDevicePtr,
            out IntPtr dxContextPtr)
        {
            var graphicsDeviceType = typeof(GraphicsDevice);

            var d3DDeviceInfo =
                graphicsDeviceType.GetField("_d3dDevice", BindingFlags.Instance | BindingFlags.NonPublic);
            var deviceObj = d3DDeviceInfo.GetValue(graphicsDevice);
            var deviceType = deviceObj.GetType();
            var devicePtrInfo = deviceType.GetProperty("NativePointer", BindingFlags.Instance | BindingFlags.Public);
            dxDevicePtr = (IntPtr) devicePtrInfo.GetValue(deviceObj);

            var d3DContextInfo =
                graphicsDeviceType.GetField("_d3dContext", BindingFlags.Instance | BindingFlags.NonPublic);
            var contextObj = d3DContextInfo.GetValue(graphicsDevice);
            var contextType = contextObj.GetType();
            var contextPtrInfo = contextType.GetProperty("NativePointer", BindingFlags.Instance | BindingFlags.Public);
            dxContextPtr = (IntPtr) contextPtrInfo.GetValue(contextObj);
        }

        public static IntPtr GetNativeDxResource(this RenderTarget2D renderTarget2D)
        {
            var renderTarget2DType = typeof(RenderTarget2D);

            var textureInfo = renderTarget2DType.GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic);
            var resourceObj = textureInfo.GetValue(renderTarget2D);
            var resourceType = resourceObj.GetType();
            var resourcePtrInfo =
                resourceType.GetProperty("NativePointer", BindingFlags.Instance | BindingFlags.Public);
            return (IntPtr) resourcePtrInfo.GetValue(resourceObj);
        }
    }
}