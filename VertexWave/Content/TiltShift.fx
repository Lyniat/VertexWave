texture ScreenTexture;
 
sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};

Texture xSSAOMap;

float xPlayerDistance;

sampler SSAOMapSampler = sampler_state { texture = <xSSAOMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

float4 TiltShift(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    float4 Color;
    
    float4 textureColor = tex2D(SSAOMapSampler, TextureCoordinate);
    
    float distance = textureColor.r;
    
    float BlurDistance = 0;//.002;
    
    if(TextureCoordinate.y > 0.7){
    //if(distance > xPlayerDistance + xPlayerDistance/40){
        BlurDistance = 0.002;
    }
    
    if(TextureCoordinate.y < 0.3){
    //if(distance < xPlayerDistance - xPlayerDistance/40){
            BlurDistance = 0.002;
        }
    
     
    // Get the texel from ColorMapSampler using a modified texture coordinate. This
    // gets the texels at the neighbour texels and adds it to Color.
   Color  = tex2D( TextureSampler, float2(TextureCoordinate.x+BlurDistance, TextureCoordinate.y+BlurDistance));
   Color += tex2D( TextureSampler, float2(TextureCoordinate.x-BlurDistance, TextureCoordinate.y-BlurDistance));
   Color += tex2D( TextureSampler, float2(TextureCoordinate.x+BlurDistance, TextureCoordinate.y-BlurDistance));
    Color += tex2D( TextureSampler, float2(TextureCoordinate.x-BlurDistance, TextureCoordinate.y+BlurDistance));
    // We need to devide the color with the amount of times we added
    // a color to it, in this case 4, to get the avg. color
    Color = Color / 4;
     
    // returned the blurred color
    return Color;
}

technique PostProcess
{
       pass P0
       {
             PixelShader = compile ps_5_0 TiltShift();
       }
}