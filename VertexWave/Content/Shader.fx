#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

float4x4 xWorldViewProjection;
//float4x4 xLightsWorldViewProjection;
//float4x4 xWorld;
//float3 xLightPos;
//float4 xAlphaTest;
float3 xCamPosition;
//float xLightPower;
//float xAmbient;
//float xTime;
//float xTransparent;
//float4 xFogColor;
//float xFogDistance;
//float xFogGradient;
//float xFogValue;

float xModeFactor;
float xStartModeFactor;

Texture xShadowMapNum;

Texture xTexture;

sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = POINT; minfilter = POINT; mipfilter=POINT; AddressU = mirror; AddressV = mirror;};

Texture xShadowMap;

sampler ShadowMapSampler = sampler_state { texture = <xShadowMap> ; magfilter = POINT; minfilter = POINT; mipfilter=POINT; AddressU = mirror; AddressV = mirror;};

Texture xSSAOMap;

sampler SSAOMapSampler = sampler_state { texture = <xSSAOMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

static const float PI = 3.14159265f;


struct SSceneVertexToPixel
{
    float4 Position             : POSITION; 
    float4 Color : COLOR0;
};

struct SScenePixelToFrame
{
    float4 Color : COLOR0;
};


 SSceneVertexToPixel ShadowedSceneVertexShader( float4 inPos : POSITION, float4 inColor : COLOR0)
 {
     SSceneVertexToPixel Output = (SSceneVertexToPixel)0;
     
	 /*
     if (inMode.x > 0){
        inPos.x  = inPos.x + (sin((inPos.x + inPos.y) * xTime/30000)) * 0.1;
        inPos.z  = inPos.z + (sin((inPos.z + inPos.y) * xTime/30000)) * 0.1;
     }

	 */

	 bool invisible = false;

	 if (inPos.y < 2) {
		 invisible = true;
	 }

	 float startColor = 60;

	 float startColorBlue = 90;

	 float endColor = 100;

	 float start = 40;

	 float startX = 50;

	 float distance = inPos.z - xCamPosition.z;

	 float distanceX = inPos.x - xCamPosition.x;

	 float loopSize = 15;

	 float curveY = inPos.y;

	 float loopY = inPos.y;

	 //CURVE

	 /*
	 if (distance < -start) {
		 curveY = curveY - ((distance + start)) * xStartModeFactor;
	 }

	 */

	 if (distance < 0) {
		curveY = curveY - (cos(distance/ 30) * 15 - 15) * (1-xStartModeFactor);
	 }

	 
	 if (distanceX < -startX) {
		 curveY = curveY - (distanceX + startX) * (1 - xStartModeFactor);;
	 }

	 if (distanceX > startX) {
		 curveY = curveY + (distanceX - startX) * (1 - xStartModeFactor);;
	 }

	
	//LOOP

	 float newY = cos(distanceX / 15);
	 float newX = sin(distanceX / 15);

	 
	 if (newY > 0) {
		 loopY = loopY - newY * 15 + 15;
	 }
	 else {
		 loopY = loopY + newY * 15 + 15;
	 }
	 
	 
	 if (distanceX / 15 > PI/2) { 
		 inColor.r = inColor.r * xModeFactor;
		 inColor.g = inColor.g * xModeFactor;
		 inColor.b = inColor.b * xModeFactor;
		 
	 }

	 if (distanceX / 15 < -PI / 2) {
		 inColor.r = inColor.r * xModeFactor;
	     inColor.g = inColor.g * xModeFactor;
	     inColor.b = inColor.b * xModeFactor;
		 
	 }

	 //COLOR

	 float val = (distance + startColor) / endColor;
	 inColor.r = inColor.r * val;
	 inColor.g = inColor.g * val;


	 float valBlue = (distance + startColorBlue) / endColor;
	 inColor.b = inColor.b * valBlue;

	 if (distance > start) {
		 inColor.r = inColor.r - (distance - start) / 20;
		 inColor.g = inColor.g - (distance - start) / 20;
		 inColor.b = inColor.b - (distance - start) / 20;
	 }

	 loopY = loopY - distance / 10;

	 curveY = curveY * xModeFactor;
	 loopY = loopY * (1-xModeFactor);

	 inPos.y = curveY + loopY;

	 if (invisible) {
		 inColor.r = 0;
			 inColor.g = 0;
			 inColor.b = 0;
	 }
 
     Output.Position = mul(inPos, xWorldViewProjection);    
     Output.Color = inColor;
 
     return Output;
 }
 
 SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
 {
	 SScenePixelToFrame Output = (SScenePixelToFrame)0;

	 Output.Color = PSIn.Color;
     return Output;
 }


technique ShadowedScene
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL ShadowedSceneVertexShader();
        PixelShader = compile PS_SHADERMODEL ShadowedScenePixelShader();
    }
}