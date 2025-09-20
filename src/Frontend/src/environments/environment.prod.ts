export const environment = {
  production: true,
  apiUrl: 'https://api.airbnb-platform.com/graphql',
  apiWsUrl: 'wss://api.airbnb-platform.com/graphql',
  appName: 'AirBnB Platform',
  version: '1.0.0',
  enableLogging: false,
  googleMapsApiKey: 'YOUR_PRODUCTION_GOOGLE_MAPS_API_KEY',
  stripePublishableKey: 'pk_live_YOUR_STRIPE_PUBLISHABLE_KEY',
  features: {
    enableNotifications: true,
    enableReviews: true,
    enablePayments: true,
    enableChat: true
  }
};