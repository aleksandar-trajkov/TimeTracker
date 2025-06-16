const executeFetch = async function (url: string, method: string, body: any | null = null, token: string | null = null): Promise<any> {
  const params = {
    method,
    headers: {
      'Content-Type': 'application/json'
    }
  }
  if (body) {
    //params.body = JSON.stringify(body);
  }
  if(token) {
    //params.headers['Authorization'] = 'Bearer ' + token;
  }
  var response = await fetch(url, params);
  return response;
};

const executeGet = async function (url: string, token: string | null = null): Promise<any> {
  return executeFetch(url, 'GET', null, token).then((response) => response.json());
}

const executePost = function (url: string, body: any, token: string | null = null): Promise<any> {
  return executeFetch(url, 'POST', body, token).then((response) => response.json());
}

const executePut = function (url: string, body: any, token: string | null = null): Promise<any> {
  return executeFetch(url, 'PUT', body, token).then((response) => response.json());
}

const executeDelete = function (url: string, token: string | null = null): Promise<any> {
  return executeFetch(url, 'DELETE', null, token);
}

export { 
  executeFetch,
  executeGet,
  executePost,
  executePut,
  executeDelete
};