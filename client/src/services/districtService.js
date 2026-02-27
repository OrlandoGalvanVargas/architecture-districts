import { districtsApi } from "../api/district/districts.api";
import { createService } from "../reactive/createService";

const districtReactor = {
  onSuccess: ({ action, payload, params, db }) => {
    switch (action) {
      case "getAll":
        db.collection("districts").bulkWrite(payload);
        break;
    }
  },
  onError: ({ action, error, params }) => {
    switch (action) {
      case "getAll":
        alert(error);
    }
  },
};

export const districtService = createService(districtsApi, districtReactor);
