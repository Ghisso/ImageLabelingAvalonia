// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ImageLabelingAvalonia.ViewModels;

namespace ImageLabelingAvalonia
{
    /// Avalonia stuff
    public class ViewLocator : IDataTemplate
    {
        /// Avalonia stuff
        public bool SupportsRecycling => false;

        /// Avalonia stuff
        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        /// Avalonia stuff
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}