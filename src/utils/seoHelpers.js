export const FAQSchemaGenerator = (faqs) => ({
  '@context': 'https://schema.org',
  '@type': 'FAQPage',
  mainEntity: faqs.map(faq => ({
    '@type': 'Question',
    name: faq.question,
    acceptedAnswer: {
      '@type': 'Answer',
      text: faq.answer
    }
  }))
});

export const ArticleSchemaGenerator = (article) => ({
  '@context': 'https://schema.org',
  '@type': 'NewsArticle',
  headline: article.title,
  description: article.description,
  image: article.image,
  datePublished: article.datePublished,
  dateModified: article.dateModified,
  author: {
    '@type': 'Organization',
    name: 'New Kenya Campaign'
  },
  publisher: {
    '@type': 'Organization',
    name: 'New Kenya Campaign',
    logo: {
      '@type': 'ImageObject',
      url: '/assets/icons/icon-512.svg'
    }
  }
});

export const BreadcrumbSchemaGenerator = (breadcrumbs) => ({
  '@context': 'https://schema.org',
  '@type': 'BreadcrumbList',
  itemListElement: breadcrumbs.map((item, index) => ({
    '@type': 'ListItem',
    position: index + 1,
    name: item.name,
    item: item.url
  }))
});

export const generateMetaTags = (data) => {
  const {
    title,
    description,
    image,
    url,
    type = 'website',
    breadcrumbs = [],
    structuredData = null,
    keywords = [],
    author = 'New Kenya Campaign',
    twitterHandle = '@newkenya',
    datePublished,
    dateModified
  } = data;

  const meta = {
    // Basic meta
    title: `${title} | New Kenya Campaign`,
    description,
    keywords: keywords.join(', '),
    author,

    // OpenGraph
    'og:title': title,
    'og:description': description,
    'og:image': image,
    'og:url': url,
    'og:type': type,

    // Twitter
    'twitter:card': 'summary_large_image',
    'twitter:site': twitterHandle,
    'twitter:title': title,
    'twitter:description': description,
    'twitter:image': image,

    // Article specific
    ...(datePublished && { 'article:published_time': datePublished }),
    ...(dateModified && { 'article:modified_time': dateModified })
  };

  // Generate structured data
  const schema = {
    ...structuredData,
    ...(breadcrumbs.length > 0 && {
      breadcrumb: BreadcrumbSchemaGenerator(breadcrumbs)
    })
  };

  return {
    meta,
    schema
  };
};