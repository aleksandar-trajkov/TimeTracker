const executeFetch = async function (url: string, method: string, body: object | null = null, token: string | null = null): Promise<Response> {
  const params = {
    method,
    headers: {
      'Content-Type': 'application/json'
    } as Record<string, string>,
    body: null as object | null
  } as RequestInit;
  if (body) {
    params.body = JSON.stringify(body);
  }
  if(token) {
    (params.headers as Record<string, string>)['Authorization'] = 'Bearer ' + token;
  }
  const response = await fetch(url, params);
  return response;
};

const executeGet = async function (url: string, token: string | null = null): Promise<Response> {
  return executeFetch(url, 'GET', null, token).then((response) => response.json());
}

const executePost = function (url: string, body: object, token: string | null = null): Promise<Response> {
  return executeFetch(url, 'POST', body, token).then((response) => response.json());
}

const executePut = function (url: string, body: object, token: string | null = null): Promise<Response> {
  return executeFetch(url, 'PUT', body, token).then((response) => response.json());
}

const executeDelete = function (url: string, token: string | null = null): Promise<Response> {
  return executeFetch(url, 'DELETE', null, token);
}

export { 
  executeFetch,
  executeGet,
  executePost,
  executePut,
  executeDelete
};