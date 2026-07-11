import { schoolsApi } from "../api/school/school.api";
import { createService } from "../reactive/createService";

const schoolReactor = {
  onSuccess: ({ action, payload, db }) => {
    switch (action) {
      case "getAll":
        db.collection("schools").bulkWrite(payload);
        break;
    }
  },
  onError: ({ action, error }) => {
    switch (action) {
      case "getAll":
        console.log("Error fetching schools: ", error);
        break;
      default:
        console.log(`Error in ${action}: `, error);
    }
  },
};

export const schoolService = createService(schoolsApi, schoolReactor);
