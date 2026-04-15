using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Domain.Midi;

namespace Kataka.App.Services;

/// <summary>
/// Owns all amp state synchronisation: pending write queues, debounced write timer,
/// periodic read-poll timer, push-notification routing, and the read/write operations
/// that talk to the amp over <see cref="IKatanaSession"/>.
///
/// The ViewModel's observable collections and properties are mutated directly here
/// (under a <c>suppressChangeTracking</c> guard so that device-initiated changes do
/// not echo back as pending writes). All UI-only concerns (status messages, diagnostic
/// log, file dialogs) stay in the ViewModel.
/// </summary>
public sealed class AmpStateService 
{
    

}
