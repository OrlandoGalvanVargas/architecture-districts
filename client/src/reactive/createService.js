const db = {
  collection: (name) => {
    return {
      bulkWrite: (data) => {
        localStorage.setItem(name, JSON.stringify(data));
      },
    };
  },
};

function createAction(client, action, service) {
  return (...params) => {
    return client[action](...params)
      .then((result) => {
        {
          service.onSuccess({ action, payload: result, params, db });
          return result;
        }
      })
      .catch((error) => service.onError(action, error, ...params));
  };
}

export function createService(client, service) {
  let newService = {};

  Object.keys(client).forEach((key) => {
    newService[key] = createAction(client, key, service);
  });
  return newService;
}
