import React from 'react';

export default class AdminErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { error: null };
    this.reset = this.reset.bind(this);
  }

  static getDerivedStateFromError(error) {
    return { error };
  }

  componentDidCatch(error, info) {
    console.error('[AdminErrorBoundary]', error, info.componentStack);
  }

  reset() {
    this.setState({ error: null });
  }

  render() {
    if (this.state.error) {
      return (
        <div className="p-12 flex flex-col items-center justify-center text-center min-h-[300px]">
          <p className="text-5xl mb-4">⚠️</p>
          <h3 className="text-lg font-bold text-gray-900 mb-2">Something went wrong in this tab</h3>
          <p className="text-sm text-gray-500 mb-6 max-w-md font-mono bg-gray-100 px-3 py-2 rounded">
            {this.state.error.message}
          </p>
          <button onClick={this.reset} className="fluent-btn fluent-btn-primary">
            Try Again
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}
