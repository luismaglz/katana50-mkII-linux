// #region Copyright
// 
// =================================================================================================
// This file is part of the Navitaire software development kit.
// Copyright © Navitaire LLC  An Amadeus company. All rights reserved.
// ==================================================================================================
// 
// #endregion

namespace Kataka.App.ViewModels;

public class ChainNode(ChainNodeType chainNodeType, string color)
{
    public ChainNodeType ChainNodeType { get; } = chainNodeType;
    public string Color { get; } = color;
}
