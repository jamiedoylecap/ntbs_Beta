import Vue from 'vue';
import { getHeaders } from '../helpers';
import axios from 'axios';

type TravelOrVisitVariables = {
    hasVisitor?: string,
    hasTravel?: string,
    totalNumberOfCountries: string,
    country1Id: string,
    country2Id: string,
    country3Id: string,
    stayLengthInMonths1: string,
    stayLengthInMonths2: string,
    stayLengthInMonths3: string
};

const ValidateTravelOrVisit = Vue.extend({
    props: {
        modelType: {
            type: String
        }
    },
    methods: {
        prependRefWithModelType: function (ref: String) {
            return `${this.$props.modelType}-${ref}`;
        },

        validate: function (event: FocusEvent) {
            if (this.$el.contains(event.relatedTarget)) {
                return;
            }

            const travelOrVisitVariables = this.getTravelOrVisitVariables();
            const requestConfig = {
                url: `Travel/Validate${this.$props.modelType}`,
                headers: getHeaders(),
                params: travelOrVisitVariables
            }

            axios.request(requestConfig)
                .catch((error: any) => {
                    console.log(error.response);
                })
                .then((response: any) => {
                    this.resetErrors();
                    const data = response.data;

                    for (let key in data) {
                        if (data.hasOwnProperty(key)) {
                            // Model errors can be double counted as 'Model.Property' and 'Property'.
                            // Here it's convenient to act on 'Property' only
                            if (key.indexOf(".") === -1) {
                                const baseRef = key.charAt(0).toLowerCase() + key.substring(1);
                                const errorRef = baseRef + "ErrorRef";
                                const formGroupRef = baseRef + "FormGroup";
                                const errorMessage = data[key];

                                const baseElement = this.$refs[baseRef];
                                if (baseElement.tagName === "INPUT") {
                                    this.$refs[baseRef].classList.add("nhsuk-input--error");
                                } else if (baseElement.tagName === "SELECT") {
                                    this.$refs[baseRef].classList.add("nhsuk-select--error");
                                }

                                this.$refs[errorRef].textContent = errorMessage;
                                this.$refs[formGroupRef].classList.add("nhsuk-form-group--error");
                            }
                        }
                    }
                });
        },

        resetErrors: function () {
            const refList = [
                "totalNumberOfCountries",
                "country1Id",
                "country2Id",
                "country3Id",
                "stayLengthInMonths1",
                "stayLengthInMonths2",
                "stayLengthInMonths3"
            ];

            for (let i = 0; i < refList.length; i++) {
                const ref = refList[i];
                const errorRef = ref + "ErrorRef";
                const formGroupRef = ref + "FormGroup";

                const baseElement = this.$refs[ref];
                if (baseElement.tagName === "INPUT") {
                    this.$refs[ref].classList.remove("nhsuk-input--error");
                } else if (baseElement.tagName === "SELECT") {
                    this.$refs[ref].classList.remove("nhsuk-select--error");
                }

                this.$refs[errorRef].textContent = "";
                this.$refs[formGroupRef].classList.remove("nhsuk-form-group--error");
            }
        },

        getTravelOrVisitVariables: function () {
            const variables: TravelOrVisitVariables = {
                totalNumberOfCountries: this.$refs["totalNumberOfCountries"].value,
                country1Id: this.$refs["country1Id"].value,
                country2Id: this.$refs["country2Id"].value,
                country3Id: this.$refs["country3Id"].value,
                stayLengthInMonths1: this.$refs["stayLengthInMonths1"].value,
                stayLengthInMonths2: this.$refs["stayLengthInMonths2"].value,
                stayLengthInMonths3: this.$refs["stayLengthInMonths3"].value
            };

            const lowerModelType = this.$props.modelType.toLowerCase();
            if (lowerModelType === "visitor") {
                variables.hasVisitor = this.getHasDataValue();
            } else if (lowerModelType === "travel") {
                variables.hasTravel = this.getHasDataValue();
            }

            return variables;
        },

        getHasDataValue: function () {
            if (this.$refs["hasDataYes"].checked) {
                return true;
            } else if (this.$refs["hasDataNo"].checked) {
                return false;
            }
            return null;
        },
    }
});

export default ValidateTravelOrVisit;