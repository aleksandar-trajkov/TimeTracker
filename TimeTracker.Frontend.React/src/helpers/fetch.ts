import { fixDateTimeForResponse } from "./dateTime";

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

const executeGet = async function<T>(url: string, token: string | null = null): Promise<T> {
  return executeFetch(url, 'GET', null, token).then((response) => response.json()).then((data) => fixDateTimeForResponse<T>(data));
}

const executePost = function<T>(url: string, body: object, token: string | null = null): Promise<T> {
  return executeFetch(url, 'POST', body, token).then((response) => response.json()).then((data) => fixDateTimeForResponse<T>(data));
}

const executePut = function<T>(url: string, body: object, token: string | null = null): Promise<T> {
  return executeFetch(url, 'PUT', body, token).then((response) => response.json()).then((data) => fixDateTimeForResponse<T>(data));
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