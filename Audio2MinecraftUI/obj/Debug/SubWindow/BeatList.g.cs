﻿#pragma checksum "..\..\..\SubWindow\BeatList.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F9B9D0F2DCCF5444AC970D0E0CC0810D6A16CAB615D1D2F09ACE9FB41F1B1BD8"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Audio2MinecraftUI.SubWindow;
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


namespace Audio2MinecraftUI.SubWindow {
    
    
    /// <summary>
    /// BeatList
    /// </summary>
    public partial class BeatList : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\SubWindow\BeatList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Add;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\SubWindow\BeatList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ListView;
        
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
            System.Uri resourceLocater = new System.Uri("/Audio2MinecraftUI;component/subwindow/beatlist.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SubWindow\BeatList.xaml"
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
            
            #line 9 "..\..\..\SubWindow\BeatList.xaml"
            ((Audio2MinecraftUI.SubWindow.BeatList)(target)).Initialized += new System.EventHandler(this.MetroWindow_Initialized);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Add = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\SubWindow\BeatList.xaml"
            this.Add.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Export);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\SubWindow\BeatList.xaml"
            this.Add.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Export);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ListView = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
