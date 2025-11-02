// Lightweight test runner using Node assert - avoids external test deps
import assert from 'assert';
import { generateMetaTags } from '../src/utils/seoHelpers.js';

function run() {
  try {
    const result = generateMetaTags({ title: 'Test', description: 'desc', image: '/img.png', url: '/test' });
    assert(result && result.meta, 'generateMetaTags should return meta object');
    if (result.meta['og:title'] !== 'Test') throw new Error('og:title should match');
    console.log('Unit tests passed');
    process.exit(0);
  } catch (err) {
    console.error('Unit tests failed:', err);
    process.exit(2);
  }
}

run();
