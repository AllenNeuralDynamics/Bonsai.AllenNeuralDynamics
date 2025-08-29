using System;
using Bonsai.Vision.Design;
using OpenTK.Graphics.OpenGL;
using Bonsai.Design;
using Bonsai.Expressions;
using AllenNeuralDynamics.VrForaging;

namespace AllenNeuralDynamics.Core.Design
{
    public class IplImageSaturationVisualizer : IplImageVisualizer
    {
        int shaderProgram;
        int textureLocation;
        int minSaturationLocation;
        int maxSaturationLocation;
        int alphaLocation;

        private byte minSaturation { get; set; } = 0;
        private byte maxSaturation { get; set; } = 255;
        public float alpha { get; set; } = 0.5f;

        public override void Load(IServiceProvider provider)
        {

            base.Load(provider);

            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            if (ExpressionBuilder.GetVisualizerElement(context.Source).Builder is IplImageSaturationVisualizerBuilder visualizerBuilder)
            {
                minSaturation = visualizerBuilder.Minimum;
                maxSaturation = visualizerBuilder.Maximum;
                alpha = visualizerBuilder.OverlayAlpha;
                VisualizerCanvas.RenderFrame += (sender, e) =>
                {
                    minSaturation = visualizerBuilder.Minimum;
                    maxSaturation = visualizerBuilder.Maximum;
                    alpha = visualizerBuilder.OverlayAlpha;
                };
            }

            VisualizerCanvas.Load += (sender, e) =>
            {
                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.PointSmooth);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                shaderProgram = CompileShader(vertexShaderCode, fragShaderCode);
                textureLocation = GL.GetUniformLocation(shaderProgram, "texture");
                minSaturationLocation = GL.GetUniformLocation(shaderProgram, "minSaturation");
                maxSaturationLocation = GL.GetUniformLocation(shaderProgram, "maxSaturation");
                alphaLocation = GL.GetUniformLocation(shaderProgram, "alpha");
            };
        }

        protected override void RenderFrame()
        {
            GL.PushMatrix();
            GL.UseProgram(shaderProgram);

            base.RenderFrame();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 1);

            GL.Uniform1(textureLocation, 0);
            GL.Uniform1(minSaturationLocation, (float)minSaturation / byte.MaxValue);
            GL.Uniform1(maxSaturationLocation, (float)maxSaturation / byte.MaxValue);
            GL.Uniform1(alphaLocation, (float)alpha);

            GL.PopMatrix();
            GL.UseProgram(0);
        }

        int CompileShader(string vertexCode, string fragmentCode)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexCode);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexCompiled);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentCode);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragmentCompiled);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int programLinked);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        const string fragShaderCode = @"
#version 330 core

in vec2 TexCoords;
uniform sampler2D imageTexture;
uniform float minSaturation = 1.0;
uniform float maxSaturation = 0.0;
uniform float alpha = 1.0;

out vec4 FragColor;

void main()
{
    vec4 texColor = texture(imageTexture, TexCoords);
    vec4 overlay = vec4(0,0,0,0);

    if (texColor.r >= maxSaturation) overlay = vec4(1,0,0,alpha);
    else if (texColor.r <= minSaturation) overlay = vec4(0,0,1,alpha);

    FragColor = mix(texColor, overlay, overlay.a);
    }
";
        const string vertexShaderCode = @"#version 330 core
layout(location = 0) in vec2 vertexPosition;
layout(location = 1) in vec2 vertexTexCoords;

out vec2 TexCoords;

void main()
{
    TexCoords = vec2(.5, -.5) * (vertexPosition.xy + 1);
    gl_Position = vec4(vertexPosition.xy, 0.0, 1.0);
}
";
    }
}
