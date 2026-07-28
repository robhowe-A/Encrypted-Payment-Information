//!--Copyright (c) Robert A. Howell  2026
export default class QueryParams {
  #urlParams;

  constructor(queryString) {
    if (queryString === null) {
      return Error('QueryString cannot be null');
    }
    this.queryString = queryString;

    this.urlParams = new URLSearchParams(this.queryString);
  };

  // Parse the query string for the desired parameter value
  getParameterValue(parameterKey) {
    if (parameterKey === null) {
      return Error('QueryString cannot be null');
    }
    return this.urlParams.get(parameterKey);
  };
}
