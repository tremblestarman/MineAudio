﻿#pragma checksum "..\..\..\SubWindow\Export.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B2C1414F22770BF0A9C2FA39C56AEB66"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ExecutiveMidi.SubWindow;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ExecutiveMidi.SubWindow {
    
    
    /// <summary>
    /// Export
    /// </summary>
    public partial class Export : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox 延伸方向;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox 序列宽度;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox 保持区块加载;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox 重设BPM;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Midi刻长;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Midi时长;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Ok;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\SubWindow\Export.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Done;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ExecutiveMidi;component/subwindow/export.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SubWindow\Export.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\SubWindow\Export.xaml"
            ((ExecutiveMidi.SubWindow.Export)(target)).Initialized += new System.EventHandler(this.Window_Initialized);
            
            #line default
            #line hidden
            
            #line 10 "..\..\..\SubWindow\Export.xaml"
            ((ExecutiveMidi.SubWindow.Export)(target)).Closed += new System.EventHandler(this.MetroWindow_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.延伸方向 = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\..\SubWindow\Export.xaml"
            this.延伸方向.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this._SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.序列宽度 = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\SubWindow\Export.xaml"
            this.序列宽度.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            
            #line 18 "..\..\..\SubWindow\Export.xaml"
            this.序列宽度.KeyDown += new System.Windows.Input.KeyEventHandler(this._KeyDown);
            
            #line default
            #line hidden
            
            #line 18 "..\..\..\SubWindow\Export.xaml"
            this.序列宽度.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this._KeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.保持区块加载 = ((System.Windows.Controls.CheckBox)(target));
            
            #line 20 "..\..\..\SubWindow\Export.xaml"
            this.保持区块加载.Click += new System.Windows.RoutedEventHandler(this._Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.重设BPM = ((System.Windows.Controls.TextBox)(target));
            
            #line 22 "..\..\..\SubWindow\Export.xaml"
            this.重设BPM.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\SubWindow\Export.xaml"
            this.重设BPM.KeyDown += new System.Windows.Input.KeyEventHandler(this._KeyDown);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\SubWindow\Export.xaml"
            this.重设BPM.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this._KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Midi刻长 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.Midi时长 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.Ok = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\..\SubWindow\Export.xaml"
            this.Ok.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OK);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\SubWindow\Export.xaml"
            this.Ok.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OK);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Done = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\..\SubWindow\Export.xaml"
            this.Done.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.DoneChanges);
            
            #line default
            #line hidden
            
            #line 38 "..\..\..\SubWindow\Export.xaml"
            this.Done.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.DoneChanges);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
