﻿using System.Net.Http;

namespace Drum
{
    public static class HttpRequestExtensions
    {
        public static UriMaker<T> TryGetUriMakerFor<T>(this HttpRequestMessage req)
        {
            var context = req.GetConfiguration().TryGetUriMakerContext();
            return context != null ? context.NewUriMakerFor<T>(req) : null;
        }

        public static UriMaker<TController> TryGetUriMaker<TController>(this TController controller, HttpRequestMessage req)
        {
            return req.TryGetUriMakerFor<TController>();
        }
    }
}