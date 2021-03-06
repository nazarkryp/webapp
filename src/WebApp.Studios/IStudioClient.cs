﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Studios
{
    public interface IStudioClient
    {
        string StudioName { get; }

        Task<int> GetPagesCountAsync();

        Task<IEnumerable<StudioMovie>> GetMoviesAsync(int page);

        Task<StudioMovie> GetMovieDetailsAsync(string movieUri);

        Task<StudioMovie> ParseDetailsAsync(string html, string movieUri);
    }
}
