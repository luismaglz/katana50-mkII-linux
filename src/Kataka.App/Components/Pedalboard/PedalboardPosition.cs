// #region Copyright
//
// =================================================================================================
// This file is part of the Navitaire software development kit.
// Copyright © Navitaire LLC  An Amadeus company. All rights reserved.
// ==================================================================================================
//
// #endregion

using Kataka.App.ViewModels;

namespace Kataka.App.Components.Pedalboard;

public abstract class PedalboardPosition
{
    protected PedalboardPosition(string color)
    {
        Color = color;
    }

    public string Color { get; }
}

public class PedalboardHardware : PedalboardPosition
{
    public PedalboardHardware(string imagePath, string color)
        : base(color)
    {
        ImagePath = imagePath;
    }

    public string ImagePath { get; set; }
}

public class PedalboardPedal<T> : PedalboardPosition where T : ViewModelBase
{
    public PedalboardPedal(T viewModel, string color) : base(color)
    {
        ViewModel = viewModel;
    }

    public T ViewModel { get; set; }
}
