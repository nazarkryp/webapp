export interface IMapper<TSource, TResult> {
    mapFromResponse(response: TSource): TResult;
    mapFromResponseArray(response: TSource[]): TResult[];
}
