// Fix for Apollo Angular TypeScript compatibility issues
declare module 'apollo-angular' {
  import { OperationVariables } from '@apollo/client/core';
  
  export interface SubscribeToMoreOptions<TData, TSubscriptionVariables extends OperationVariables, TSubscriptionData> {
    document: any;
    variables?: TSubscriptionVariables;
    updateQuery?: (prev: TData, options: { subscriptionData: { data: TSubscriptionData } }) => TData;
    onError?: (err: Error) => void;
  }

  export interface UpdateQueryOptions<TData, TVariables extends OperationVariables> {
    variables?: TVariables;
    context?: any;
  }
}