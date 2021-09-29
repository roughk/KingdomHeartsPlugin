﻿using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using KingdomHeartsPlugin.Utilities;
using System;
using System.Numerics;
using KingdomHeartsPlugin.UIElements.HealthBar;

namespace KingdomHeartsPlugin
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    public class PluginUI : IDisposable
    {
        internal Configuration Configuration;
        private readonly HealthFrame _healthFrame;
        /*private TextureWrap _testTextureWrap;
        private float _width;
        private float _height;
        private float[] pos;
        private float[] pos2;
        private float[] uv;
        private float[] uv2;*/

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = true;
        public bool Visible
        {
            get => this.visible;
            set => this.visible = value;
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get => this.settingsVisible;
            set => this.settingsVisible = value;
        }

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration)
        {
            this.Configuration = configuration;
            _healthFrame = new HealthFrame();

            /*_testTextureWrap = KingdomHeartsPlugin.Pi.UiBuilder.LoadImage(Path.Combine(KingdomHeartsPlugin.TemplateLocation, @"Textures\LimitGauge\number_2.png"));
            pos = new float[4];
            pos2 = new float[4];
            uv = new float[4];
            uv2 = new float[4];
            _width = 256;
            _height = 256;*/
        }

        public void Dispose()
        {
            _healthFrame?.Dispose();
            ImageDrawing.Dispose();
            //_testTextureWrap?.Dispose();
        }

        public void OnUpdate()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
            DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }


            ImGuiWindowFlags window_flags = 0;
            window_flags |= ImGuiWindowFlags.NoTitleBar;
            window_flags |= ImGuiWindowFlags.NoScrollbar;
            if (Configuration.Locked)
            {
                window_flags |= ImGuiWindowFlags.NoMove;
                window_flags |= ImGuiWindowFlags.NoMouseInputs;
                window_flags |= ImGuiWindowFlags.NoNav;
            }
            window_flags |= ImGuiWindowFlags.AlwaysAutoResize;
            window_flags |= ImGuiWindowFlags.NoBackground;

            var size = new Vector2(320, 320);
            ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowSizeConstraints(size, new Vector2(float.MaxValue, float.MaxValue));
            
            if (ImGui.Begin("KH Frame", ref this.visible, window_flags))
            {
                _healthFrame.Draw();
            }
            ImGui.End();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(232, 500), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Kingdom Hearts Bars: Configuration", ref this.settingsVisible,
               ImGuiWindowFlags.NoCollapse))
            {
                ImGui.BeginTabBar("KhTabBar");
                if (ImGui.BeginTabItem("General"))
                {
                    // can't ref a property, so use a local copy
                    var enabled = this.Configuration.Locked;
                    if (ImGui.Checkbox("Locked", ref enabled))
                    {
                        this.Configuration.Locked = enabled;
                        // can save immediately on change, if you don't want to provide a "Save and Close" button
                    }

                    var scale = this.Configuration.Scale;
                    if (ImGui.InputFloat("Scale", ref scale, 0.025f, 0.1f))
                    {
                        Configuration.Scale = scale;
                        if (Configuration.Scale < 0.25f)
                            Configuration.Scale = 0.25f;
                        if (Configuration.Scale > 3)
                            Configuration.Scale = 3;
                    }

                    /*ImGui.NewLine();
                    ImGui.Separator();

                    ImGui.SliderFloat("Width", ref _width, 0, 512);
                    ImGui.SliderFloat("Height", ref _height, 0, 512);
                    ImGui.SliderFloat("Pos[0]", ref pos[0], 0, 256);
                    ImGui.SliderFloat("Pos[1]", ref pos[1], 0, 256);
                    ImGui.SliderFloat("Pos[2]", ref pos[2], 0, 256);
                    ImGui.SliderFloat("Pos[3]", ref pos[3], 0, 256);
                    ImGui.SliderFloat("Pos2[0]", ref pos2[0], 0, 256);
                    ImGui.SliderFloat("Pos2[1]", ref pos2[1], 0, 256);
                    ImGui.SliderFloat("Pos2[2]", ref pos2[2], 0, 256);
                    ImGui.SliderFloat("Pos2[3]", ref pos2[3], 0, 256);
                    ImGui.SliderFloat("UV[0]", ref uv[0], 0, 1);
                    ImGui.SliderFloat("UV[1]", ref uv[1], 0, 1);
                    ImGui.SliderFloat("UV[2]", ref uv[2], 0, 1);
                    ImGui.SliderFloat("UV[3]", ref uv[3], 0, 1);
                    ImGui.SliderFloat("UV2[0]", ref uv2[0], 0, 1);
                    ImGui.SliderFloat("UV2[1]", ref uv2[1], 0, 1);
                    ImGui.SliderFloat("UV2[2]", ref uv2[2], 0, 1);
                    ImGui.SliderFloat("UV2[3]", ref uv2[3], 0, 1);

                    ImGui.NewLine();

                    //ImGui.Image(_testTextureWrap.ImGuiHandle, new Vector2(pos[0], pos[1]), new Vector2(uv[0], uv[1]), new Vector2(uv[2], uv[3]));

                    var dl = ImGui.GetWindowDrawList();
                    ImGui.Dummy(new Vector2(_width, _height));
                    double width = _testTextureWrap.Width;
                    double height = _testTextureWrap.Height;
                    Vector2 position = ImGui.GetItemRectMin();

                    dl.PushClipRect(position - new Vector2(0, 0), position + new Vector2(_width, _height));
                    dl.AddImageQuad(_testTextureWrap.ImGuiHandle, 
                        position + new Vector2((pos[0]), (pos[1])), 
                        position + new Vector2((pos[2]), (pos[3])),
                        position + new Vector2((pos2[0]), (pos2[1])),
                        position + new Vector2((pos2[2]), (pos2[3]))/*,
                        position + new Vector2((uv[0]), (uv[1])), 
                        position + new Vector2((uv[2]), (uv[3])),
                        position + new Vector2((uv2[0]), (uv2[1])),
                        position + new Vector2((uv2[2]), (uv2[3]))
                        );
                    dl.PopClipRect();*/

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Health"))
                {
                    var fullRing = Configuration.HpForFullRing;
                    if (ImGui.InputInt("HP for full ring", ref fullRing, 5, 50))
                    {
                        Configuration.HpForFullRing = fullRing;
                        if (Configuration.HpForFullRing < 1)
                            Configuration.HpForFullRing = 1;
                    }

                    var hpPerPixel = Configuration.HpPerPixelLongBar;
                    if (ImGui.InputFloat("HP per pixel for long bar", ref hpPerPixel, 5, 50))
                    {
                        Configuration.HpPerPixelLongBar = hpPerPixel;
                        if (Configuration.HpPerPixelLongBar < 0.0001f)
                            Configuration.HpPerPixelLongBar = 0.0001f;
                    }

                    var maxLength = Configuration.MaximumHpForMaximumLength;
                    if (ImGui.InputInt("HP for maximum total length", ref maxLength, 5, 50))
                    {
                        Configuration.MaximumHpForMaximumLength = maxLength;
                        if (Configuration.MaximumHpForMaximumLength < 1)
                            Configuration.MaximumHpForMaximumLength = 1;
                    }

                    var minLength = Configuration.MinimumHpForLength;
                    if (ImGui.InputInt("HP for minimum length", ref minLength, 5, 50))
                    {
                        Configuration.MinimumHpForLength = minLength;
                        if (Configuration.MinimumHpForLength < 1)
                            Configuration.MinimumHpForLength = 1;
                    }

                    var lowHpPercent = Configuration.LowHpPercent;
                    if (ImGui.SliderFloat("Percent To Trigger Low HP", ref lowHpPercent, 0, 100))
                    {
                        Configuration.LowHpPercent = lowHpPercent;
                    }

                    var truncate = Configuration.TruncateHp;
                    if (ImGui.Checkbox("Truncate HP Text Value", ref truncate))
                    {
                        Configuration.TruncateHp = truncate;
                    }
                    if (ImGui.IsItemHovered())
                    {
                        Vector2 m = ImGui.GetIO().MousePos;
                        ImGui.SetNextWindowPos(new Vector2(m.X + 20, m.Y + 20));
                        ImGui.Begin("TT1", ImGuiWindowFlags.Tooltip | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);
                        ImGui.Text("Truncate HP value over 10000 to 10.0K and 100000 to 100K");
                        ImGui.End();
                    }
                    
                    var showHpVal = Configuration.ShowHpVal;
                    if (ImGui.Checkbox("Show HP Value", ref showHpVal))
                    {
                        Configuration.ShowHpVal = showHpVal;
                    }

                    var showHpRecovery = Configuration.ShowHpRecovery;
                    if (ImGui.Checkbox("Show HP Recovery", ref showHpRecovery))
                    {
                        Configuration.ShowHpRecovery = showHpRecovery;
                    }
                    if (ImGui.IsItemHovered())
                    {
                        Vector2 m = ImGui.GetIO().MousePos;
                        ImGui.SetNextWindowPos(new Vector2(m.X + 20, m.Y + 20));
                        ImGui.Begin("TT2", ImGuiWindowFlags.Tooltip | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);
                        ImGui.Text("Shows a blue bar for when HP is recovered then gradually fills the green bar.");
                        ImGui.End();
                    }

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("MP/GP/CP"))
                {
                    ImGui.Text("Position");
                    var resourcePos = new Vector2(Configuration.ResourceBarPositionX, Configuration.ResourceBarPositionY);
                    if (ImGui.DragFloat2("Position (X, Y)", ref resourcePos))
                    {
                        Configuration.ResourceBarPositionX = resourcePos.X;
                        Configuration.ResourceBarPositionY = resourcePos.Y;
                    }

                    ImGui.Separator();
                    ImGui.NewLine();
                    ImGui.Text("Value Text");
                    ImGui.Separator();

                    var showVal = Configuration.ShowResourceVal;
                    if (ImGui.Checkbox("Show Resource Value", ref showVal))
                    {
                        Configuration.ShowResourceVal = showVal;
                    }
                    var resourceTextPos = new Vector2(Configuration.ResourceTextPositionX, Configuration.ResourceTextPositionY);
                    if (ImGui.DragFloat2("Text Position (X, Y)", ref resourceTextPos))
                    {
                        Configuration.ResourceTextPositionX = resourceTextPos.X;
                        Configuration.ResourceTextPositionY = resourceTextPos.Y;
                    }

                    ImGui.Separator();
                    ImGui.NewLine();
                    ImGui.Text("MP");
                    ImGui.Separator();

                    var mpPerPixel = Configuration.MpPerPixelLength;
                    if (ImGui.InputFloat("MP per pixel for bar length", ref mpPerPixel, 0.1f, 0.5f, "%f"))
                    {
                        Configuration.MpPerPixelLength = mpPerPixel;
                        if (Configuration.MpPerPixelLength < 0.0001f)
                            Configuration.MpPerPixelLength = 0.0001f;
                    }

                    var maximumMpLength = Configuration.MaximumMpLength;
                    if (ImGui.InputInt("MP for maximum length", ref maximumMpLength, 1, 25))
                    {
                        Configuration.MaximumMpLength = maximumMpLength;
                        if (Configuration.MaximumMpLength < 1)
                            Configuration.MaximumMpLength = 1;
                    }

                    var minimumMpLength = Configuration.MinimumMpLength;
                    if (ImGui.InputInt("MP for minimum length", ref minimumMpLength, 1, 25))
                    {
                        Configuration.MinimumMpLength = minimumMpLength;
                        if (Configuration.MinimumMpLength < 1)
                            Configuration.MinimumMpLength = 1;
                    }

                    var truncate = Configuration.TruncateMp;
                    if (ImGui.Checkbox("Truncate MP Value", ref truncate))
                    {
                        Configuration.TruncateMp = truncate;
                    }
                    if (ImGui.IsItemHovered())
                    {
                        Vector2 m = ImGui.GetIO().MousePos;
                        ImGui.SetNextWindowPos(new Vector2(m.X + 20, m.Y + 20));
                        ImGui.Begin("TT1", ImGuiWindowFlags.Tooltip | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);
                        ImGui.Text("Truncate MP from 10000 to 100.");
                        ImGui.End();
                    }

                    ImGui.Separator();
                    ImGui.NewLine();
                    ImGui.Text("GP");
                    ImGui.Separator();

                    var gpPerPixel = Configuration.GpPerPixelLength;
                    if (ImGui.InputFloat("GP per pixel for bar length", ref gpPerPixel, 0.1f, 0.5f, "%f"))
                    {
                        Configuration.GpPerPixelLength = gpPerPixel;
                        if (Configuration.GpPerPixelLength < 0.0001f)
                            Configuration.GpPerPixelLength = 0.0001f;
                    }

                    var maximumGpLength = Configuration.MaximumGpLength;
                    if (ImGui.InputInt("GP for maximum length", ref maximumGpLength, 1, 25))
                    {
                        Configuration.MaximumGpLength = maximumGpLength;
                        if (Configuration.MaximumGpLength < 1)
                            Configuration.MaximumGpLength = 1;
                    }

                    var minimumGpLength = Configuration.MinimumGpLength;
                    if (ImGui.InputInt("GP for minimum length", ref minimumGpLength, 1, 25))
                    {
                        Configuration.MinimumGpLength = minimumGpLength;
                        if (Configuration.MinimumGpLength < 1)
                            Configuration.MinimumGpLength = 1;
                    }

                    ImGui.Separator();
                    ImGui.NewLine();
                    ImGui.Text("CP");
                    ImGui.Separator();

                    var cpPerPixel = Configuration.CpPerPixelLength;
                    if (ImGui.InputFloat("CP per pixel for bar length", ref cpPerPixel, 0.1f, 0.5f, "%f"))
                    {
                        Configuration.CpPerPixelLength = cpPerPixel;
                        if (Configuration.CpPerPixelLength < 0.0001f)
                            Configuration.CpPerPixelLength = 0.0001f;
                    }

                    var maximumCpLength = Configuration.MaximumCpLength;
                    if (ImGui.InputInt("CP for maximum length", ref maximumCpLength, 1, 25))
                    {
                        Configuration.MaximumCpLength = maximumCpLength;
                        if (Configuration.MaximumCpLength < 1)
                            Configuration.MaximumCpLength = 1;
                    }

                    var minimumCpLength = Configuration.MinimumCpLength;
                    if (ImGui.InputInt("CP for minimum length", ref minimumCpLength, 1, 25))
                    {
                        Configuration.MinimumCpLength = minimumCpLength;
                        if (Configuration.MinimumCpLength < 1)
                            Configuration.MinimumCpLength = 1;
                    }

                    ImGui.EndTabItem();
                }


                if (ImGui.BeginTabItem("Limit Gauge"))
                {
                    var limitAlwaysShow = Configuration.LimitGaugeAlwaysShow;
                    if (ImGui.Checkbox("Always Show", ref limitAlwaysShow))
                    {
                        Configuration.LimitGaugeAlwaysShow = limitAlwaysShow;
                    }
                    var limitPosX = Configuration.LimitGaugePositionX;
                    if (ImGui.InputFloat("X Position", ref limitPosX, 1, 25))
                    {
                        Configuration.LimitGaugePositionX = limitPosX;
                    }
                    var limitPosY = Configuration.LimitGaugePositionY;
                    if (ImGui.InputFloat("Y Position", ref limitPosY, 1, 25))
                    {
                        Configuration.LimitGaugePositionY = limitPosY;
                    }

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
                ImGui.Separator();
                if (ImGui.Button("Save"))
                {
                    this.Configuration.Save();
                }
            }
            ImGui.End();
        }

    }
}
