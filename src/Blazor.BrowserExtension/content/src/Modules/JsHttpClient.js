export async function Fetch(url, init) {
  const response = await fetch(url, init);
  return new FetchResponse(response);
}

class FetchResponse {
  /**
   * Creates a new instance of FetchResponse
   * @param {Response} response
   */
  constructor(response) {
    this.response = response;
  }

  GetStatusCode() {
    return this.response.status;
  }

  GetResponseText() {
    return this.response.text();
  }

  GetStatusText() {
    return this.response.statusText;
  }
}