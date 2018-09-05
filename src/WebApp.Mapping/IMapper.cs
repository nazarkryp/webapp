﻿using System.Collections.Generic;

namespace WebApp.Mapping
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        TDestination Map<TDestination>(object source);

        TDestination Map<TDestination>(Dictionary<string, object> properties) where TDestination : new();
    }
}
