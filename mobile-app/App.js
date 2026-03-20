import { StatusBar } from 'expo-status-bar';
import { useEffect, useState } from 'react';
import { SafeAreaView, ScrollView, StyleSheet, Text, View } from 'react-native';

const API_BASE = 'http://localhost:5065/api';

export default function App() {
  const [status, setStatus] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    async function loadStatus() {
      try {
        const response = await fetch(`${API_BASE}/results/status`);
        if (!response.ok) {
          throw new Error('Failed to load election status');
        }
        const data = await response.json();
        setStatus(data);
      } catch (err) {
        setError(err.message || 'Network error');
      }
    }

    loadStatus();
  }, []);

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView contentContainerStyle={styles.content}>
        <Text style={styles.title}>Project H Mobile Command</Text>
        <Text style={styles.subtitle}>Mobile-first companion for field operations</Text>

        {error ? <Text style={styles.error}>{error}</Text> : null}

        <View style={styles.card}>
          <Text style={styles.cardTitle}>Election Status Snapshot</Text>
          {!status ? (
            <Text style={styles.cardBody}>Loading...</Text>
          ) : (
            <>
              <Text style={styles.cardBody}>Window: {status.reportingWindow}</Text>
              <Text style={styles.cardBody}>Submitted: {status.submitted}</Text>
              <Text style={styles.cardBody}>Validated: {status.validated}</Text>
              <Text style={styles.cardBody}>Pending: {status.pending}</Text>
              <Text style={styles.cardBody}>Rejected: {status.rejected}</Text>
            </>
          )}
        </View>
      </ScrollView>
      <StatusBar style="auto" />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8fafc'
  },
  content: {
    padding: 24,
    gap: 16
  },
  title: {
    fontSize: 28,
    fontWeight: '700',
    color: '#0f172a'
  },
  subtitle: {
    fontSize: 14,
    color: '#334155'
  },
  card: {
    backgroundColor: '#ffffff',
    borderRadius: 16,
    padding: 16,
    borderColor: '#e2e8f0',
    borderWidth: 1
  },
  cardTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#0f172a',
    marginBottom: 8
  },
  cardBody: {
    fontSize: 14,
    color: '#334155',
    marginBottom: 4
  },
  error: {
    color: '#b91c1c'
  }
});
