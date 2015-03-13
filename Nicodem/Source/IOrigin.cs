﻿namespace Nicodem.Source.Tmp
{
    public interface IOrigin
    {
        /// <summary>
        ///     Location representing beginning of this source.
        /// </summary>
        ILocation Begin { get; }

        /// <summary>
        ///     Returns reader of this origin, which is set to the Begin.
        /// </summary>
        IOriginReader GetReader();

        IFragment MakeFragment(ILocation from, ILocation to);
    }
}